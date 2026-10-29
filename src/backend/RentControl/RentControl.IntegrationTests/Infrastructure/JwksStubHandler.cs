using System.Net;
using System.Text;

namespace RentControl.IntegrationTests.Infrastructure;

/// <summary>
/// Заглушка JWKS-сервера Main.Api: любой запрос возвращает JWKS тестового ключа.
/// Подставляется primary-обработчиком в named-клиент "jwt-jwks", чтобы сервис
/// валидировал токены без запущенного Main.Api (соседний сервис мокается на
/// HTTP-границе — это уровень интеграционных тестов).
/// </summary>
public sealed class JwksStubHandler(TestJwtKeys keys) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(keys.ToJwksJson(), Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
