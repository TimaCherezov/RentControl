# Подсчёт покрытия тестами

Два сценария: покрытие юнитами (по классам) и совокупное покрытие всеми уровнями пирамиды. Оба считаются локально; в CI покрытие не собирается.

## Разовая установка инструментов

```bash
dotnet tool install -g dotnet-coverage
dotnet tool install -g dotnet-reportgenerator-globaltool
```

`coverlet.collector` уже подключён в `RentControl.UnitTests` и `RentControl.IntegrationTests`.

## Покрытие юнит-тестами

```bash
dotnet test src/backend/RentControl/RentControl.UnitTests -c Release --collect:"XPlat Code Coverage"
```

Отчёт появится в `TestResults/<guid>/coverage.cobertura.xml` — структура: `packages` → `package` (сборка) → `class` c атрибутом `line-rate`; парсится напрямую.

Быстрая сводка по сборкам и классам:

```bash
reportgenerator \
  -reports:"src/backend/RentControl/RentControl.UnitTests/TestResults/*/coverage.cobertura.xml" \
  -targetdir:"src/backend/RentControl/RentControl.UnitTests/TestResults/coverage-report" \
  -reporttypes:TextSummary \
  -assemblyfilters:"+Main.Api;+Main.Model;+Main.Worker;+Payments.Api;+Payments.Model;+Realtime.Api;+Realtime.Model;+RentControl.Common;+RentControl.ServiceDefaults" \
  -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*;-WeatherForecast"
```

Итог — `Summary.txt` внутри `targetdir`: общий процент и разбивка по классам.

## Совокупное покрытие (юниты + интеграционные + e2e)

e2e стартует сервисы через Aspire отдельными процессами, поэтому coverlet из тест-хоста сервисы не видит. Нужен `dotnet-coverage` — он инструментирует всю сессию, включая дочерние процессы сервисов. Интеграционным тестам и e2e нужен запущенный Docker (Testcontainers).

Грабля: у `dotnet-coverage` работают только короткие опции `-f`/`-o`; длинные `--format`/`--output` ломают парсер — он принимает `--format` за имя запускаемого процесса.

```bash
OUT=src/backend/RentControl/TestResults/coverage-combined
mkdir -p "$OUT"

dotnet-coverage collect -f cobertura -o "$OUT/unit.cobertura.xml" \
  "dotnet test src/backend/RentControl/RentControl.UnitTests -c Release"
dotnet-coverage collect -f cobertura -o "$OUT/integration.cobertura.xml" \
  "dotnet test src/backend/RentControl/RentControl.IntegrationTests -c Release"
dotnet-coverage collect -f cobertura -o "$OUT/e2e.cobertura.xml" \
  "dotnet test src/backend/RentControl/RentControl.E2ETests -c Release"

dotnet-coverage merge -o "$OUT/merged.cobertura.xml" -f cobertura \
  "$OUT/unit.cobertura.xml" "$OUT/integration.cobertura.xml" "$OUT/e2e.cobertura.xml"
```

Уровень не нужен — убери его из `collect` и `merge`.

## Сводка

```bash
reportgenerator \
  -reports:"$OUT/merged.cobertura.xml" \
  -targetdir:"$OUT/report" \
  -reporttypes:TextSummary \
  -assemblyfilters:"+Main.Api;+Main.Model;+Main.Worker;+Payments.Api;+Payments.Model;+Realtime.Api;+Realtime.Model;+RentControl.Common;+RentControl.ServiceDefaults" \
  -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*;-WeatherForecast"
```

- `assemblyfilters` обязателен: в merged-отчёт попадают все загруженные сборки (тест-фреймворки, NuGet); без фильтра итог падает до ~24%. Тестовые проекты и `RentControl.AppHost` (dev-оркестрация, не прод-код) в фильтры не включаем.
- `classfilters` убирает сгенерированный и шаблонный код из подсчёта: source-gen OpenAPI (`Microsoft.AspNetCore.OpenApi.Generated*`, поддержка XML-комментариев), компиляторные артефакты (`System.Runtime.CompilerServices*`) и остаток шаблона `WeatherForecast`. Без него картина врёт: генерация занимает больше половины Payments.Api, и сборка выглядит на ~45% вместо ~99% по бизнес-коду. Ловушка паттерна: классы называются ровно `Microsoft.AspNetCore.OpenApi.Generated` (без суффикса), поэтому маска — `...Generated*`, а не `...Generated.*`.
- Результат — `$OUT/report/Summary.txt`: общий процент по проектам решения и разбивка по классам.

## Нюансы

- Числа coverlet и dotnet-coverage для одного и того же кода слегка расходятся (по-разному считают «покрываемые строки»). Не смешивай их в одном отчёте; для сравнения прогонов бери один инструмент.
- e2e склонен к флейту (#68): упавший тест не мешает сбору — `dotnet-coverage` пишет отчёт и после неуспешного прогона.
- Ориентир для самопроверки (2026-11-26): совокупно ~89% строк и ~86% веток по бизнес-кодам решения; юнит-only: Main.Api ~60%, Payments.Api ~41%, Realtime.Api ~33%.
