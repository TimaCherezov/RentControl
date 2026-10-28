#!/usr/bin/env bash
# Установка OpenCode в кешируемый каталог. Если actions/cache уже вернул
# каталог с нужной версией — установка пропускается, в PATH добавляется
# готовый бинарник.
#
# Входы (env): OPENCODE_VERSION (мажор, например "2")
set -euo pipefail

if [ ! -x /tmp/opencode-cli/node_modules/.bin/opencode ]; then
  npm install --prefix /tmp/opencode-cli "@opencode/cli@${OPENCODE_VERSION}"
fi
echo "/tmp/opencode-cli/node_modules/.bin" >> "$GITHUB_PATH"
opencode --version
