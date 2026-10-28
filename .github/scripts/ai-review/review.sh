#!/usr/bin/env bash
# Запуск ревью: opencode run со скиллом pr-review.
# Ревьюер сам смотрит репозиторий (чекаут содержит полную историю) и
# сравнивает ветку с origin/<BASE_REF>; результаты пишет в файлы из
# REVIEW_REPORT (markdown-отчёт) и REVIEW_COMMENTS (JSON построчных
# комментариев) — их публикует publish.sh.
#
# Входы (env): ZHIPU_API_KEY, REVIEW_MODEL, PR_NUMBER, BASE_REF,
#              REVIEW_REPORT, REVIEW_COMMENTS, GH_TOKEN
set -euo pipefail

if [ -z "${ZHIPU_API_KEY:-}" ]; then
  echo "::warning::ZHIPU_API_KEY не задан — ревью пропущено."
  exit 0
fi
opencode run --standalone --auto --model "$REVIEW_MODEL" \
  "@pr-review Сделай ревью пулл-реквеста №${PR_NUMBER}: сам посмотри репозиторий и сравни эту ветку с origin/${BASE_REF} (полная история уже в чекауте). Пройди весь цикл по инструкции скилла; отчёт и комментарии запиши в файлы из \$REVIEW_REPORT и \$REVIEW_COMMENTS."
