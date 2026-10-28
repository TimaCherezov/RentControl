#!/usr/bin/env bash
# Гейт перед ревью: по sha закоммиченного билда находим PR и решаем,
# нужно ли ревью. Вызывается из .github/workflows/ai-review.yml (job resolve).
#
# Зачем искать по sha: событие workflow_run приходит на ЛЮБОЙ завершённый
# Build (пуш в main, перегон, упавший билд) и не содержит номера PR.
# Фильтры ниже не дают запускать LLM впустую: только открытые PR не-форки,
# не черновики, только актуальная голова и ещё не отревьюенное.
#
# Входы (env):
#   GH_TOKEN            - токен для gh api
#   WR_SHA              - head_sha билда
#   CLAIM_STALE_MINUTES - через сколько зависший pending считается мёртвым
# Выходы: GITHUB_OUTPUT <- pr, sha, base
set -euo pipefail
pr="" ; sha="" ; reason="" ; base=""

# Только открытые PR; для форк-PR sha может быть недоступен
# (gh вернёт ошибку в stdout) — валидируем число и тихо пропускаем.
pr="$(gh api "repos/${GITHUB_REPOSITORY}/commits/${WR_SHA}/pulls" \
  --jq '[.[] | select(.state == "open")][0].number // empty' 2>/dev/null || true)"
if ! [[ "$pr" =~ ^[0-9]+$ ]]; then
  echo "Билд ${WR_SHA:0:12} не связан с открытым PR — ревью не требуется."
  pr=""
fi

if [ -n "$pr" ]; then
  meta="$(gh api "repos/${GITHUB_REPOSITORY}/pulls/${pr}" \
    --jq '{sha: .head.sha, fork: (.head.repo.fork // false), draft: .draft, state: .state, base: .base.ref}')"
  sha="$(jq -r '.sha' <<<"$meta")"
  fork="$(jq -r '.fork' <<<"$meta")"
  draft="$(jq -r '.draft' <<<"$meta")"
  prstate="$(jq -r '.state' <<<"$meta")"
  base="$(jq -r '.base' <<<"$meta")"

  if [ "$fork" != "false" ]; then
    reason="PR из форка"
  elif [ "$prstate" != "open" ]; then
    reason="PR ${prstate}"
  elif [ "$draft" = "true" ]; then
    reason="черновик"
  elif [ -n "${WR_SHA:-}" ] && [ "$sha" != "$WR_SHA" ]; then
    reason="голова уже ${sha:0:12} — ревью придёт после билда нового коммита"
  fi

  if [ -z "$reason" ]; then
    latest="$(gh api "repos/${GITHUB_REPOSITORY}/commits/${sha}/statuses" \
      --jq '[.[] | select(.context == "ai-review")] | sort_by(.created_at) | last // empty')"
    if [ -n "$latest" ]; then
      state="$(jq -r '.state' <<<"$latest")"
      if [ "$state" = "success" ]; then
        reason="ревью ${sha:0:12} уже готово"
      elif [ "$state" = "pending" ]; then
        age=$(( ( $(date +%s) - $(date -d "$(jq -r '.created_at' <<<"$latest")" +%s) ) / 60 ))
        if [ "$age" -lt "$CLAIM_STALE_MINUTES" ]; then
          reason="ревью ${sha:0:12} уже в работе (${age} мин)"
        fi
      fi
    fi
  fi

  if [ -n "$reason" ]; then
    echo "PR #${pr}: ${reason} — пропускаем."
  else
    {
      echo "pr=${pr}"
      echo "sha=${sha}"
      echo "base=${base}"
    } >> "$GITHUB_OUTPUT"
    echo "PR #${pr}: коммит ${sha:0:12} (база origin/${base}) — запускаем ревью."
  fi
fi
