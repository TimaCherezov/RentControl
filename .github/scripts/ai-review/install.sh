#!/usr/bin/env bash
# Установка OpenCode в кешируемый каталог.
#
# Нюансы:
#  - npm на раннерах блокирует postinstall-скрипты пакетов (allowScripts),
#    а без postinstall у @opencode/cli не появляется бинарник. Поэтому
#    ставим с --ignore-scripts и запускаем postinstall.mjs вручную —
#    он выбирает платформенный бинарник и кладёт его в bin/opencode.exe.
#  - GITHUB_PATH действует только для СЛЕДУЮЩИХ шагов, поэтому проверка
#    версии внутри этого шага идёт по полному пути.
#
# Входы (env): OPENCODE_VERSION (мажор, например "2")
set -euo pipefail

BIN=/tmp/opencode-cli/node_modules/@opencode/cli/bin/opencode.exe
if [ ! -x "$BIN" ]; then
  npm install --prefix /tmp/opencode-cli --ignore-scripts "@opencode/cli@${OPENCODE_VERSION}"
  node /tmp/opencode-cli/node_modules/@opencode/cli/postinstall.mjs
  chmod +x "$BIN"
fi
echo "/tmp/opencode-cli/node_modules/.bin" >> "$GITHUB_PATH"
"$BIN" --version
