#!/usr/bin/env bash
# Заявка на ревью: ставит commit status pending (context `ai-review`) на
# head-коммит PR. Другие запуски видят pending и не стартуют второе ревью;
# снимается финальным статусом в finalize.sh (success/error).
#
# Входы (env): GH_TOKEN, HEAD_SHA
#   (+ стандартные GITHUB_REPOSITORY, GITHUB_SERVER_URL, GITHUB_RUN_ID)
set -euo pipefail

gh api --method POST "repos/${GITHUB_REPOSITORY}/statuses/${HEAD_SHA}" \
  -f state=pending -f context=ai-review \
  -f description="OpenCode: ревью запущено" \
  -f target_url="${GITHUB_SERVER_URL}/${GITHUB_REPOSITORY}/actions/runs/${GITHUB_RUN_ID}"
