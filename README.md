# RentControl

## CI/CD

GitHub Actions, всё в одном файле — [.github/workflows/build.yml](.github/workflows/build.yml). Триггеры: любой pull request и пуш в `main`.

| Job | Что делает | Блокирует мерж |
|---|---|---|
| `build` | `dotnet restore` + `dotnet build` (Release) | да |
| `e2e-tests` | `dotnet test` e2e-проекта | нет |
| `docker-build` | Сборка docker-образов 4 сервисов (матрица) | нет |
| `ai-review` | AI-ревью PR через [OpenCodeReview](https://github.com/alibaba/open-code-review), модель `glm-5.3-flash` (GLM Coding Plan от z.ai) | нет |

Порядок: `build` → `e2e-tests` → `docker-build` → (`ai-review` параллельно с двумя последними, но тоже строго после `build`).

### AI-ревью

- Публикует inline-комментарии и summary прямо в PR; сбои ревью мерж не блокируют (`continue-on-error`).
- Работает только для PR в `main` (открытие и новые коммиты); мерж в `main` ревью не запускает.
- Новый пуш в PR отменяет незаконченное ревью и ревьюет весь diff от merge-base заново.
- Требуется секрет **`ZAI_API_KEY`** (API-ключ GLM Coding Plan) в *Settings → Secrets and variables → Actions*. Нет секрета — job проходит с warning, ревью пропускается.
- Версии запинены: action — на commit SHA (`v1.13.3`), CLI — `ocr_version: '1.13.3'` (при обновлении менять оба).

**Как отключить ревью для PR:** добавь `[skip-ai-review]` в название PR — job уйдёт в skipped (токены не тратятся). Тег проверяется в момент события: добавил тег в название после старта рана — ревью досмотрит, но следующий пуш уже пропустит. Убрал тег — следующий пуш снова запустит ревью.

Перезапустить ревью вручную: *Re-run jobs* у проверки `ai-review` (или любой пустой коммит в PR).
