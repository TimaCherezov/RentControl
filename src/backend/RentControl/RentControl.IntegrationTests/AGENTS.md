# Интеграционные тесты

## Назначение

Интеграционные тесты проверяют **один сервис вместе с его реальными зависимостями** (БД, брокер). Соседние микросервисы не поднимаются — их мокаем на HTTP-границе. Если сценарий требует запущенного соседнего сервиса, это e2e (см. `RentControl.E2ETests`).

Уровни пирамиды:

- **юнит** — бизнес-логика внутри сервиса;
- **интеграционные** — сам сервис и его зависимости (БД, брокер) — этот проект;
- **e2e** — межсервисное взаимодействие (`RentControl.E2ETests`).

## Инфраструктура

Приложение целиком поднимается в памяти через `WebApplicationFactory` (`Infrastructure/IntegrationTestFactory<TProgram>`), внешние зависимости — реальные контейнеры Testcontainers:

- `Main/Infrastructure/MainApiFixture` — Main.Api + Postgres (`maindb`); миграции `Main.Model` накатываются при старте фикстуры;
- `Payments/Infrastructure/PaymentsApiFixture` — Payments.Api + Postgres (`paymentsdb`); схема создаётся по модели (`EnsureCreated`), миграций у `Payments.Model` пока нет. Зависимость от JWKS Main.Api мокается заглушкой `Infrastructure/JwksStubHandler` на named-клиенте `jwt-jwks`;
- `Realtime/Infrastructure/RealtimeApiFixture` — Realtime.Api без внешних зависимостей (пока они не появились).

Брокер (RabbitMQ) не поднимается отдельной фикстурой: когда код сервиса начнёт его использовать, контейнер стартует внутри фикстуры этого сервиса (`TestImages.RabbitMq`).

Ключи JWT: каждая фикстура генерирует эфемерную RSA-пару (`Infrastructure/TestJwtKeys`), PEM подставляется в конфиг сервиса — тесты могут выпускать токены напрямую (`CreateAccessToken`) в обход register/login.

## Структура проекта

```
RentControl.IntegrationTests/
  Infrastructure/            — общая инфраструктура (фабрика, JWT-ключи, заглушки, образы)
  Main/
    Infrastructure/          — инфраструктура Main.Api (MainApiFixture)
    MainApiSmokeTests.cs     — тесты Main.Api
  Payments/
    Infrastructure/          — инфраструктура Payments.Api (PaymentsApiFixture)
    PaymentsApiSmokeTests.cs — тесты Payments.Api
  Realtime/
    Infrastructure/          — инфраструктура Realtime.Api (RealtimeApiFixture)
    RealtimeApiSmokeTests.cs — тесты Realtime.Api
  TestCollections.cs         — определения коллекций фикстур
  AGENTS.md                  — этот файл
```

Тесты сервиса — в папке сервиса; фикстура и всё, что нужно только этому сервису — в `<Сервис>/Infrastructure/`; переиспользуемое между сервисами — в корневой `Infrastructure/`.

## Как писать тест

Самый частый случай — добавить тестовый класс в коллекцию сервиса, фикстура придёт через конструктор:

```csharp
[Collection(MainApiCollection.Name)]
public class MyFeatureTests(MainApiFixture fixture)
{
    [Fact]
    public async Task MyScenario()
    {
        var response = await fixture.Client.GetAsync("/weatherforecast");
        // ...
    }
}
```

- `fixture.Client` — готовый `HttpClient` к сервису;
- `fixture.CreateDbContext()` — контекст БД для сидирования и проверок;
- `fixture.CreateAccessToken(userId)` — токен без прохождения register/login;
- `fixture.ConnectionString` — строка подключения к тестовой БД.

Альтернатива: `IClassFixture<MainApiFixture>` — если нужен изолированный контейнер на конкретный тестовый класс (дороже по времени старта, используйте осознанно).

## Технические правила

- Тесты каждого сервиса — в свою коллекцию (`TestCollections.cs`); разные коллекции идут параллельно, классы внутри коллекции — последовательно.
- Конфигурацию сервиса переопределять только через `IntegrationTestFactory` (in-memory config поверх appsettings), порты и адреса не захардкоживать.
- Соседние сервисы не поднимаются: JWKS Main.Api и прочие HTTP-зависимости мокаются подменой `HttpMessageHandler` (пример — `JwksStubHandler` в `PaymentsApiFixture`).
- Каждый тест создаёт свои данные с уникальными идентификаторами (`int-{Guid:N}@test.local`); внутри одной коллекции тесты разделяют БД.
- Никаких `Thread.Sleep`: фабрика готова к моменту создания клиента, контейнеры ждут своего старта внутри фикстур.
- `public partial class Program` в конце `Program.cs` каждого сервиса нужен для `WebApplicationFactory` — не удалять.
- Смоук-тесты (`<Сервис>/<Сервис>ApiSmokeTests.cs`) — это и регрессия подъёма инфраструктуры, и образец использования фикстур.

> **Правило:** если изменил инфраструктуру тестов (состав контейнеров, фикстуры, схема подъёма) — обнови этот файл.
