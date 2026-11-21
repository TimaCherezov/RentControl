#!/usr/bin/env bash
# ── Снос деплоя RentControl ──────────────────────────────────────────────────
# Удаляет namespace rentcontrol со всем содержимым:
# подами, сервисами и ДАННЫМИ (PVC с БД/брокером/картинками уничтожаются).
#
# Запуск из корня репозитория:  ./k8s/teardown.sh
# Сам кластер minikube остаётся запущенным; снести и его целиком можно
# командой `minikube delete` (не вызывается здесь автоматически).
set -euo pipefail

kubectl delete namespace rentcontrol --ignore-not-found
echo "── Namespace rentcontrol удалён. Для полного сноса кластера: minikube delete"
