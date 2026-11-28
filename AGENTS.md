# RentControl

Микросервисное .NET-приложение на Aspire.

## Структура

- `src/backend/RentControl/` — solution (`RentControl.sln`):
  - `Main.Api`, `Payments.Api`, `Realtime.Api` — HTTP-сервисы;
  - `Main.Worker` — фоновый воркер;
  - `*.Model` — модели, миграции и репозитории соответствующих сервисов;
  - `RentControl.AppHost` — Aspire-точка запуска всего стека (сервисы, Postgres, RabbitMQ, фронт);
  - `RentControl.ServiceDefaults` — общее для настройки конечных сервисов: метрики, логирование, авторизация и т.д.
  - `RentControl.Common` — общие для модели;
  - `RentControl.E2ETests` — e2e-тесты.
- `src/frontend/RentControl/` — фронт (React + Vite).

## Сборка и запуск

```bash
dotnet build src/backend/RentControl/RentControl.sln
dotnet run --project src/backend/RentControl/RentControl.AppHost
```

AppHost поднимает и фронт: vite dev-сервер с прокси `/api` → Main.Api, `/api/invoices` → Payments.Api (цели прокси Aspire передаёт через env, ссылка на фронт — в дашборде). Для фронта нужен Node.js; `npm install` Aspire делает сам при старте. Состав сервисов задаётся конфигом `Services` (csv): по умолчанию стартуют все; при наличии `front` AppHost сам поднимет `main-api` и `payments-api`. Под `dotnet run` состав определяется launch-профилем — env из шелла профиль перекрывает, поэтому фронт сейчас включается только профилем `all`.

## Тестирование

Пирамида:

- **юнит** — бизнес-логика внутри сервиса;
- **интеграционные** — сам сервис и его зависимости (БД, брокер, миграции);
- **e2e** — межсервисное взаимодействие. Пишутся только для сценариев через несколько сервисов.

E2E-тесты живут в `src/backend/RentControl/RentControl.E2ETests`; правила их написания — в [RentControl.E2ETests/AGENTS.md](src/backend/RentControl/RentControl.E2ETests/AGENTS.md).

Посчитать покрытие тестами (юнитами по классам или совокупно всеми уровнями) — рецепт в [docs/coverage.md](docs/coverage.md).

Поменял контракт или стык между сервисами — прогони e2e локально перед PR:

```bash
dotnet test src/backend/RentControl/RentControl.E2ETests
```
