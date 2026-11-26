using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Main.Model.Models;
using RentControl.E2ETests.Utils;
using Xunit;

namespace RentControl.E2ETests;

/// <summary>
/// Путь оплаты: Main создаёт бронь и публикует OrderCreated → Payments выставляет
/// счёт (InvoiceCreated) → Main отмечает бронь готовой к оплате → клиент получает
/// ссылку на оплату в Payments и оплачивается → PaymentSucceeded → Main
/// подтверждает бронь.
/// </summary>
[Collection(AppHostCollection.Name)]
public sealed class BookingPaymentFlowTests(AppHostFixture fixture)
{
    [Fact]
    public async Task Booking_BecomesConfirmedAfterFakePay()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var clientToken = await TestUsers.RegisterAndGetAccessTokenAsync(fixture.MainApi, cancellationToken);
        var adminToken = await fixture.GetAdminAccessTokenAsync(cancellationToken);
        var catalog = await TestCatalogCreator.CreateAsync(fixture.MainApi, adminToken, cancellationToken);

        var booking = await TestBookings.CreateAsync(fixture.MainApi, clientToken, catalog, cancellationToken);
        var bookingId = booking.GetProperty("id").GetGuid();

        Assert.True(
            (int)BookingStatusEnum.Pending == booking.GetProperty("bookingStatus").GetInt32(),
            "Только что созданная бронь должна быть в статусе Pending");
        Assert.False(
            booking.GetProperty("isInvoiceCreated").GetBoolean(),
            "Только что созданная бронь не должна иметь счёта");
        Assert.True(
            booking.GetProperty("depositAmount").GetDecimal() > 0,
            "У брони должен быть положительный депозит для выставления счёта");

        var readyBooking = await WaitForInvoiceCreatedAsync(clientToken, bookingId);
        Assert.True(
            readyBooking.GetProperty("isInvoiceCreated").GetBoolean(),
            $"Payments не выставил счёт по брони {bookingId}: isInvoiceCreated не стал true");

        var paymentUrl = await GetPaymentUrlAsync(clientToken, bookingId, cancellationToken);
        Assert.Matches(
            @"^/fake-pay/[0-9a-f-]{36}$",
            paymentUrl);

        var paid = await FakePayAsync(paymentUrl, cancellationToken);
        Assert.True(
            "Paid" == paid.GetProperty("status").GetString(),
            "Оплата счёта не прошла");
        Assert.True(
            bookingId == paid.GetProperty("bookingId").GetGuid(),
            "Счёт оплачен не по той брони");

        var confirmedBooking = await WaitForStatusAsync(
            clientToken, bookingId, BookingStatusEnum.Confirmed,
            $"Main не подтвердил бронь {bookingId} после оплаты счёта");
        Assert.True(
            (int)BookingStatusEnum.Confirmed == confirmedBooking.GetProperty("bookingStatus").GetInt32(),
            $"Бронь {bookingId} должна быть подтверждена после оплаты");
    }

    [Fact]
    public async Task Booking_RepeatedPaymentsAreRejected()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var clientToken = await TestUsers.RegisterAndGetAccessTokenAsync(fixture.MainApi, cancellationToken);
        var adminToken = await fixture.GetAdminAccessTokenAsync(cancellationToken);
        var catalog = await TestCatalogCreator.CreateAsync(fixture.MainApi, adminToken, cancellationToken);

        var booking = await TestBookings.CreateAsync(fixture.MainApi, clientToken, catalog, cancellationToken);
        var bookingId = booking.GetProperty("id").GetGuid();

        await WaitForInvoiceCreatedAsync(clientToken, bookingId);

        var paymentUrl = await GetPaymentUrlAsync(clientToken, bookingId, cancellationToken);
        var paid = await FakePayAsync(paymentUrl, cancellationToken);
        Assert.True(
            bookingId == paid.GetProperty("bookingId").GetGuid(),
            "Счёт оплачен не по той брони");

        using var repeatedPayRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/invoices/by-booking/{bookingId}/pay");
        repeatedPayRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);

        using var repeatedPay = await fixture.PaymentsApi.SendAsync(repeatedPayRequest, cancellationToken);
        Assert.True(
            HttpStatusCode.Conflict == repeatedPay.StatusCode,
            $"Повторный запрос ссылки на оплату оплаченного счёта должен вернуть 409, пришло {(int)repeatedPay.StatusCode}");

        var invoiceId = paymentUrl["/fake-pay/".Length..];
        using var repeatedFakePay = await fixture.PaymentsApi.GetAsync(
            $"api/invoices/fake-pay/{invoiceId}",
            cancellationToken);
        Assert.True(
            HttpStatusCode.Conflict == repeatedFakePay.StatusCode,
            $"Повторная оплата уже оплаченного счёта должна вернуть 409, пришло {(int)repeatedFakePay.StatusCode}");

        var confirmedBooking = await WaitForStatusAsync(
            clientToken, bookingId, BookingStatusEnum.Confirmed,
            $"Main не подтвердил бронь {bookingId} после оплаты счёта");
        Assert.True(
            (int)BookingStatusEnum.Confirmed == confirmedBooking.GetProperty("bookingStatus").GetInt32(),
            $"Повторные оплаты не должны менять статус брони {bookingId}");
    }

    [Fact]
    public async Task Pay_ReturnsNotFoundForBookingWithoutInvoice()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var clientToken = await TestUsers.RegisterAndGetAccessTokenAsync(fixture.MainApi, cancellationToken);
        var unknownBookingId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/invoices/by-booking/{unknownBookingId}/pay");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);

        using var response = await fixture.PaymentsApi.SendAsync(request, cancellationToken);
        Assert.True(
            HttpStatusCode.NotFound == response.StatusCode,
            $"Payments должен вернуть 404 для брони {unknownBookingId} без счёта, пришло {(int)response.StatusCode}");
    }

    private async Task<JsonElement> WaitForInvoiceCreatedAsync(string accessToken, Guid bookingId)
    {
        return (await Polling.WaitUntilAsync(
            async ct =>
            {
                var booking = await TestBookings.GetByIdAsync(fixture.MainApi, accessToken, bookingId, ct);
                JsonElement? result =
                    booking.GetProperty("isInvoiceCreated").GetBoolean() ? booking : null;
                return result;
            },
            $"Payments не выставил счёт по брони {bookingId}: isInvoiceCreated не стал true")).Value;
    }

    private async Task<JsonElement> WaitForStatusAsync(
        string accessToken,
        Guid bookingId,
        BookingStatusEnum status,
        string failureDescription)
    {
        return (await Polling.WaitUntilAsync(
            async ct =>
            {
                var booking = await TestBookings.GetByIdAsync(fixture.MainApi, accessToken, bookingId, ct);
                JsonElement? result =
                    booking.GetProperty("bookingStatus").GetInt32() == (int)status ? booking : null;
                return result;
            },
            failureDescription)).Value;
    }

    private async Task<string> GetPaymentUrlAsync(
        string accessToken,
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/invoices/by-booking/{bookingId}/pay");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await fixture.PaymentsApi.SendAsync(request, cancellationToken);
        Assert.True(response.IsSuccessStatusCode,
            $"Payments не вернул ссылку на оплату по брони {bookingId}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync(cancellationToken)}");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var paymentUrl = body.GetProperty("paymentUrl").GetString();

        Assert.False(string.IsNullOrWhiteSpace(paymentUrl),
            $"Payments вернул пустую ссылку на оплату по брони {bookingId}");
        return paymentUrl!;
    }

    private async Task<JsonElement> FakePayAsync(string paymentUrl, CancellationToken cancellationToken)
    {
        using var response = await fixture.PaymentsApi.GetAsync($"api/invoices{paymentUrl}", cancellationToken);
        Assert.True(response.IsSuccessStatusCode,
            $"Оплата по ссылке {paymentUrl} не прошла: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync(cancellationToken)}");

        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }
}
