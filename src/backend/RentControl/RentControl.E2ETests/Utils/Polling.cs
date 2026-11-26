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
        var deadline = DateTimeOffset.UtcNow + (timeout ?? TimeSpan.FromSeconds(60));
        var delay = interval ?? TimeSpan.FromMilliseconds(500);

        while (true)
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var result = await probe(cancellationToken);

            if (result is not null)
            {
                return result;
            }

            if (DateTimeOffset.UtcNow >= deadline)
            {
                throw new TimeoutException(
                    $"Условие не выполнено за {timeout ?? TimeSpan.FromSeconds(60)}: {failureDescription}");
            }

            await Task.Delay(delay, cancellationToken);
        }
    }
}
