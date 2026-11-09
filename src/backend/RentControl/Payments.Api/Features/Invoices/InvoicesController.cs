using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Features.Invoices.FakePay;
using Payments.Api.Features.Invoices.Pay;

namespace Payments.Api.Features.Invoices;

[ApiController]
[Authorize]
[Route("api/invoices")]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    [HttpPost("by-booking/{bookingId:guid}/pay")]
    public async Task<IActionResult> PayByBooking(Guid bookingId, CancellationToken ct)
    {
        var client = mediator.CreateRequestClient<PayInvoiceCommand>();
        var response = await client.GetResponse<PayInvoiceResponse>(new PayInvoiceCommand(bookingId), ct);
        return Ok(response.Message);
    }
    
    [HttpGet("fake-pay/{invoiceId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> FakePay(Guid invoiceId, CancellationToken ct)
    {
        var client = mediator.CreateRequestClient<FakePayInvoiceCommand>();
        var response = await client.GetResponse<FakePayInvoiceResponse>(new FakePayInvoiceCommand(invoiceId), ct);
        return Ok(new { status = "Paid", bookingId = response.Message.BookingId });
    }
}
