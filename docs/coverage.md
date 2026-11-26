# Подсчёт покрытия тестами

Основной отчёт — таблица по проектам: покрытие юнитами и всеми тестами отдельными колонками, строки и ветки, без кода, на который мы не влияем (сгенерированное, шаблонное, EF-миграции). Оба сценария считаются локально; в CI покрытие не собирается.

## Разовая установка инструментов

```bash
dotnet tool install -g dotnet-coverage
dotnet tool install -g dotnet-reportgenerator-globaltool
```

`coverlet.collector` уже подключён в `RentControl.UnitTests` и `RentControl.IntegrationTests`.

## Сбор сырых данных

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

## Основной отчёт — таблица по проектам

Считается по собранным выше XML; запускать из корня репозитория. Обе колонки (юниты и все тесты) берутся из `dotnet-coverage` — один инструмент, числа внутри таблицы сравнимы между собой.

```bash
python -I - <<'EOF'
import xml.etree.ElementTree as ET
from fnmatch import fnmatch

EXCLUDE = ['Microsoft.AspNetCore.OpenApi.Generated*', 'System.Runtime.CompilerServices*',
           '*.Migrations*']
PROJECTS = ['Main.Api', 'Main.Model', 'Main.Worker', 'Payments.Api', 'Payments.Model',
            'Realtime.Api', 'Realtime.Model', 'RentControl.Common', 'RentControl.ServiceDefaults']

def stats(path):
    res = {}
    for pkg in ET.parse(path).iter('package'):
        lv = lc = bv = bc = 0
        for cls in pkg.iter('class'):
            if any(fnmatch(cls.get('name', ''), p) for p in EXCLUDE):
                continue
            for ln in cls.iter('line'):
                lv += 1
                if ln.get('hits') != '0':
                    lc += 1
                if ln.get('branch') == 'True':
                    b, t = ln.get('condition-coverage', '(0/0)').split('(')[1].split(')')[0].split('/')
                    bv += int(t); bc += int(b)
        res[pkg.get('name')] = (lc, lv, bc, bv)
    return res

def cell(c, t):
    return f"{100*c/t:.1f}% ({c}/{t})" if t else "—"

u = stats('src/backend/RentControl/TestResults/coverage-combined/unit.cobertura.xml')
c = stats('src/backend/RentControl/TestResults/coverage-combined/merged.cobertura.xml')
tot = [0]*8
print('| Проект | Юниты — строки | Юниты — ветки | Все тесты — строки | Все тесты — ветки |')
print('|---|---|---|---|---|')
for p in PROJECTS:
    row = u.get(p, (0,)*4) + c.get(p, (0,)*4)
    print(f'| {p} | {cell(row[0],row[1])} | {cell(row[2],row[3])} | {cell(row[4],row[5])} | {cell(row[6],row[7])} |')
    tot = [a+b for a, b in zip(tot, row)]
print(f'| **Итого** | {cell(tot[0],tot[1])} | {cell(tot[2],tot[3])} | {cell(tot[4],tot[5])} | {cell(tot[6],tot[7])} |')
EOF
```

Что исключено и почему:

- `assemblyfilters` (зашит в `PROJECTS`): только прод-сборки решения; тестовые проекты и `RentControl.AppHost` (dev-оркестрация, не прод-код) не включаем. Без фильтра в merged попадают все загруженные сборки (тест-фреймворки, NuGet) и итог падает до ~24%.
- `classfilters` (`EXCLUDE`): source-gen OpenAPI (`Microsoft.AspNetCore.OpenApi.Generated*` — сборка OpenAPI-документа и XML-комментарии), компиляторные артефакты (`System.Runtime.CompilerServices*`), EF-миграции и снапшоты модели (`*.Migrations*`). Всё это генерируется тулингом — тестами на него не влияем; без исключений картина врёт в обе стороны: генерация занимает больше половины Payments.Api (сборка выглядела на ~45% вместо ~99%), а хорошо «покрываемые» миграции маскируют реальные дыры (Main.Model выглядела на ~94% при репозиториях в 4–43%).
- Ловушка паттерна: классы называются ровно `Microsoft.AspNetCore.OpenApi.Generated` (без суффикса), поэтому маска `...Generated*`, а не `...Generated.*`.

Ориентир для самопроверки (2026-11-26; прогон 208 юнитов + 16 интеграционных + 7 e2e, всё зелёное):

| Проект | Юниты — строки | Юниты — ветки | Все тесты — строки | Все тесты — ветки |
|---|---|---|---|---|
| Main.Api | 61.9% (2510/4058) | 69.3% (280/404) | 78.1% (3168/4058) | 87.6% (298/340) |
| Main.Model | 10.4% (76/731) | 17.6% (24/136) | 67.6% (494/731) | 42.9% (48/112) |
| Main.Worker | 59.4% (82/138) | 40.0% (8/20) | 100.0% (138/138) | 100.0% (20/20) |
| Payments.Api | 40.9% (144/352) | 32.1% (18/56) | 98.9% (348/352) | 100.0% (52/52) |
| Payments.Model | 2.7% (2/74) | 0.0% (0/8) | 94.6% (70/74) | 100.0% (8/8) |
| Realtime.Api | 33.3% (36/108) | 0.0% (0/16) | 81.5% (88/108) | 50.0% (8/16) |
| Realtime.Model | 100.0% (6/6) | — | 100.0% (6/6) | — |
| RentControl.Common | 22.0% (26/118) | 0.0% (0/8) | 64.4% (76/118) | 25.0% (2/8) |
| RentControl.ServiceDefaults | 6.9% (24/348) | 2.4% (2/84) | 94.8% (330/348) | 100.0% (52/52) |
| **Итого** | **49.0%** (2906/5933) | **45.4%** (332/732) | **79.5%** (4718/5933) | **80.3%** (488/608) |

## Быстрый просмотр по классам

Вспомогательный drill-down по классам; итоговые проценты из него не сравнивать с таблицей основного отчёта (ReportGenerator агрегирует ветки иначе, а юнит-прогон coverlet'ом считает «покрываемые строки» не так, как dotnet-coverage).

Юниты (coverlet, ~1 минута, Docker не нужен):

```bash
dotnet test src/backend/RentControl/RentControl.UnitTests -c Release --collect:"XPlat Code Coverage"
```

Отчёт появится в `TestResults/<guid>/coverage.cobertura.xml` — структура: `packages` → `package` (сборка) → `class` c атрибутом `line-rate`; парсится напрямую. Сводка по сборкам и классам:

```bash
reportgenerator \
  -reports:"src/backend/RentControl/RentControl.UnitTests/TestResults/*/coverage.cobertura.xml" \
  -targetdir:"src/backend/RentControl/RentControl.UnitTests/TestResults/coverage-report" \
  -reporttypes:TextSummary \
  -assemblyfilters:"+Main.Api;+Main.Model;+Main.Worker;+Payments.Api;+Payments.Model;+Realtime.Api;+Realtime.Model;+RentControl.Common;+RentControl.ServiceDefaults" \
  -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*;-*.Migrations*;-*.Migrations.*"
```

Итог — `Summary.txt` внутри `targetdir`.

Совокупно (по merged из сбора выше):

```bash
reportgenerator \
  -reports:"$OUT/merged.cobertura.xml" \
  -targetdir:"$OUT/report" \
  -reporttypes:TextSummary \
  -assemblyfilters:"+Main.Api;+Main.Model;+Main.Worker;+Payments.Api;+Payments.Model;+Realtime.Api;+Realtime.Model;+RentControl.Common;+RentControl.ServiceDefaults" \
  -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*;-System.Runtime.CompilerServices*;-*.Migrations*;-*.Migrations.*"
```

Итог — `$OUT/report/Summary.txt`.

## Нюансы

- Числа coverlet и dotnet-coverage для одного и того же кода заметно расходятся (по-разному считают «покрываемые строки» и ветки: юнит-only Main.Api — 59.9% у coverlet против 61.9% у dotnet-coverage, Main.Model — 21.4% против 10.4%). Не смешивай инструменты в одном отчёте: основной таблице это не грозит, она целиком на dotnet-coverage.
- e2e склонен к флейту (#68): упавший тест не мешает сбору — `dotnet-coverage` пишет отчёт и после неуспешного прогона.
