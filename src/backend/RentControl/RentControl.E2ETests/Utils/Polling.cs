using Xunit;

namespace RentControl.E2ETests.Utils;

/// <summary>
/// Ожидание асинхронных событий (обработка сообщений RabbitMQ воркером) через
/// периодический опрос с дедлайном: без Thread.Sleep и бесконечных ожиданий.
/// </summary>
public static class Polling
{
    public static async Task<T> WaitUntilAsync<T>(
        Func<CancellationToken, Task<T>> probe,
        string failureDescription,
        TimeSpan? timeout = null,
        TimeSpan? interval = null)
    {
        var effectiveTimeout = timeout ?? TimeSpan.FromSeconds(60);
        var deadline = DateTimeOffset.UtcNow + effectiveTimeout;
        var delay = interval ?? TimeSpan.FromMilliseconds(500);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        timeoutCts.CancelAfter(effectiveTimeout);

        while (true)
        {
            var result = await probe(timeoutCts.Token);

            if (result is not null)
            {
                return result;
            }

            if (DateTimeOffset.UtcNow >= deadline)
            {
                throw new TimeoutException(
                    $"Условие не выполнено за {effectiveTimeout}: {failureDescription}");
            }

            await Task.Delay(delay, timeoutCts.Token);
        }
    }
}
