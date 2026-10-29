# RentControl

Микросервисное .NET-приложение на Aspire.

## Структура

- `src/backend/RentControl/` — solution (`RentControl.sln`):
  - `Main.Api`, `Payments.Api`, `Realtime.Api` — HTTP-сервисы;
  - `Main.Worker` — фоновый воркер;
  - `*.Model` — модели, миграции и репозитории соответствующих сервисов;
  - `RentControl.AppHost` — Aspire-точка запуска всего стека (сервисы, Postgres, RabbitMQ);
  - `RentControl.ServiceDefaults` — общее для настройки конечных сервисов: метрики, логирование, авторизация и т.д.
  - `RentControl.Common` — общие для модели;
  - `RentControl.E2ETests` — e2e-тесты.

## Сборка и запуск

```bash
dotnet build src/backend/RentControl/RentControl.sln
dotnet run --project src/backend/RentControl/RentControl.AppHost
```

## Тестирование

Пирамида:

- **юнит** — бизнес-логика внутри сервиса;
- **интеграционные** — сам сервис и его зависимости (БД, брокер, миграции);
- **e2e** — межсервисное взаимодействие. Пишутся только для сценариев через несколько сервисов.

E2E-тесты живут в `src/backend/RentControl/RentControl.E2ETests`; правила их написания — в [RentControl.E2ETests/AGENTS.md](src/backend/RentControl/RentControl.E2ETests/AGENTS.md).

Поменял контракт или стык между сервисами — прогони e2e локально перед PR:

```bash
dotnet test src/backend/RentControl/RentControl.E2ETests
```

## Пул-реквесты

PR в `main` проходит AI-ревью. Если дифф больше ~500 строк — добавь `[skip-ai-review]` в название PR, чтобы не тратить токены на ревью. Агент, создающий большой PR, ставит тег сам и уведомляет пользователя, что ревью пропущено.
