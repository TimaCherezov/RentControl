#!/usr/bin/env bash
# Публикация результата AI-ревью в пулл-реквест.
# Вызывается из .github/workflows/ai-review.yml.
#
# Входы (env):
#   GITHUB_REPOSITORY - owner/repo (стандартная переменная Actions)
#   PR_NUMBER         - номер пулл-реквеста
#   HEAD_SHA          - SHA проверяемого коммита (привязка inline-комментариев)
#   REVIEW_MODEL      - модель для подписи
#   REVIEW_REPORT     - путь к markdown-отчёту (сводный комментарий)
#   REVIEW_COMMENTS   - путь к JSON-массиву построчных комментариев
#   MAX_COMMENTS      - лимит построчных комментариев (по умолчанию 10)
set -euo pipefail

: "${PR_NUMBER:?PR_NUMBER is required}"
: "${HEAD_SHA:?HEAD_SHA is required}"
report="${REVIEW_REPORT:-}"
comments="${REVIEW_COMMENTS:-}"
max_comments="${MAX_COMMENTS:-10}"

sha_short="${HEAD_SHA:0:12}"
footer="---
<sub>🤖 Автоматическое ревью [OpenCode](https://opencode.ai) · модель \`${REVIEW_MODEL:-zai/glm-4.7}\` · коммит \`${sha_short}\` · запускается через 30 минут после последнего пуша в PR</sub>"

# 1. Сводный комментарий с отчётом.
if [ -n "$report" ] && [ -s "$report" ]; then
  tmp="$(mktemp)"
  {
    cat "$report"
    printf '\n%s\n' "$footer"
  } > "$tmp"
  gh pr comment "$PR_NUMBER" --repo "$GITHUB_REPOSITORY" --body-file "$tmp"
  rm -f "$tmp"
else
  echo "::warning::REVIEW_REPORT отсутствует или пуст — сводный комментарий не публикуем."
fi

# 2. Построчные комментарии через REST API.
if [ -z "$comments" ] || [ ! -s "$comments" ]; then
  echo "Файл построчных комментариев отсутствует или пуст — пропускаем."
  exit 0
fi

if ! jq -e 'type == "array"' "$comments" > /dev/null; then
  echo "::warning::REVIEW_COMMENTS не является JSON-массивом — построчные комментарии пропускаем."
  exit 0
fi

posted=0
skipped=0
while IFS= read -r item; do
  if [ "$posted" -ge "$max_comments" ]; then
    echo "Достигнут лимит в $max_comments построчных комментариев."
    break
  fi

  file="$(jq -r '.file // empty' <<<"$item")"
  line="$(jq -r '.line // empty' <<<"$item")"
  severity="$(jq -r '.severity // "info"' <<<"$item")"
  body="$(jq -r '.body // empty' <<<"$item")"

  if [ -z "$file" ] || [ -z "$body" ] || ! [[ "$line" =~ ^[0-9]+$ ]]; then
    skipped=$((skipped + 1))
    continue
  fi

  case "$severity" in
    critical) badge="🔴" ;;
    warning)  badge="🟡" ;;
    *)        badge="🔵" ;;
  esac

  payload="$(jq -n \
    --arg body "${badge} ${body}" \
    --arg path "$file" \
    --arg line "$line" \
    --arg commit "$HEAD_SHA" \
    '{body: $body, path: $path, side: "RIGHT", line: ($line | tonumber), commit_id: $commit}')"

  if gh api --method POST "repos/${GITHUB_REPOSITORY}/pulls/${PR_NUMBER}/comments" --input - <<<"$payload" > /dev/null; then
    echo "Опубликовано: ${file}:${line}"
    posted=$((posted + 1))
  else
    echo "::warning::Не удалось опубликовать комментарий ${file}:${line} (строка вне diff?)"
    skipped=$((skipped + 1))
  fi
done < <(jq -c '.[]' "$comments")

echo "Построчных комментариев опубликовано: $posted, пропущено: $skipped."
