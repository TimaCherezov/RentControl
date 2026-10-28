#!/usr/bin/env bash
# Финальный commit status на head-коммите: success, если job прошёл целиком
# (ревью опубликовано), иначе error (следующий успешный билд или ручной
# force-запуск попробуют снова).
#
# Входы (env): GH_TOKEN, HEAD_SHA, JOB_STATUS (success|failure|cancelled)
#   (+ стандартные GITHUB_REPOSITORY, GITHUB_SERVER_URL, GITHUB_RUN_ID)
set -euo pipefail

if [ "$JOB_STATUS" = "success" ]; then
  gh api --method POST "repos/${GITHUB_REPOSITORY}/statuses/${HEAD_SHA}" \
    -f state=success -f context=ai-review -f description="Ревью опубликовано" \
    -f target_url="${GITHUB_SERVER_URL}/${GITHUB_REPOSITORY}/actions/runs/${GITHUB_RUN_ID}"
else
  gh api --method POST "repos/${GITHUB_REPOSITORY}/statuses/${HEAD_SHA}" \
    -f state=error -f context=ai-review -f description="Ревью не удалось — повторится после следующего успешного билда" \
    -f target_url="${GITHUB_SERVER_URL}/${GITHUB_REPOSITORY}/actions/runs/${GITHUB_RUN_ID}"
fi
