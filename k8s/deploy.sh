#!/usr/bin/env bash
# Сборка образов и деплой RentControl в minikube. Запуск из корня репозитория.
set -euo pipefail

cd "$(dirname "$0")/.."

NS=rentcontrol
SERVICES=(Main.Api Payments.Api Main.Worker Realtime.Api)
TIMEOUT=600s

command -v minikube >/dev/null || { echo "✗ minikube не найден — см. k8s/README.md"; exit 1; }
command -v kubectl  >/dev/null || { echo "✗ kubectl не найден — см. k8s/README.md"; exit 1; }

if ! minikube status >/dev/null 2>&1; then
  echo "── Запускаю кластер minikube (первый раз — несколько минут)..."
  # runtime=docker: kubelet берёт образы из того же демона, куда собираем;
  # ports: Docker Desktop не маршрутизирует IP ноды к хосту, публикуем на localhost
  minikube start --driver=docker --cpus=4 --memory=6g --container-runtime=docker \
    --ports=30080:30080 --ports=30081:30081 --ports=30082:30082 --ports=30672:30672
fi

eval "$(minikube -p minikube docker-env --shell bash)"
for svc in "${SERVICES[@]}"; do
  tag="rentcontrol/$(echo "$svc" | tr '[:upper:]' '[:lower:]' | tr '.' '-')"
  echo "── Собираю образ $tag:local ..."
  docker build -f "src/backend/RentControl/$svc/Dockerfile" -t "$tag:local" src/backend/RentControl
done

kubectl apply -f k8s/00-namespace.yaml
kubectl apply -f k8s/05-config.yaml

echo "── Применяю postgres и rabbitmq..."
kubectl apply -f k8s/10-postgres.yaml
kubectl apply -f k8s/15-rabbitmq.yaml
kubectl -n "$NS" rollout status --timeout="$TIMEOUT" deployment/postgres
kubectl -n "$NS" rollout status --timeout="$TIMEOUT" deployment/rabbitmq

echo "── Применяю сервисы..."
kubectl apply -f k8s/20-main-api.yaml
kubectl apply -f k8s/25-payments-api.yaml
kubectl apply -f k8s/30-main-worker.yaml
kubectl apply -f k8s/35-realtime-api.yaml
for d in main-api payments-api main-worker realtime-api; do
  kubectl -n "$NS" rollout status --timeout="$TIMEOUT" "deployment/$d"
done

echo
echo "════════════════ Готово ════════════════"
echo " Main.Api      http://localhost:30080   (/health)"
echo " Payments.Api  http://localhost:30081"
echo " Realtime.Api  http://localhost:30082/alive"
echo " RabbitMQ UI   http://localhost:30672   (rentcontrol / rc-rabbit-pass)"
echo
echo " Поды:         kubectl -n ${NS} get pods"
echo " Логи:         kubectl -n ${NS} logs deploy/main-api -f"
echo " Снести всё:   ./k8s/teardown.sh"
