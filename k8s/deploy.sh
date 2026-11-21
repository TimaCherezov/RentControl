#!/usr/bin/env bash
# ── Деплой RentControl в minikube ────────────────────────────────────────────
# 1) поднимает кластер (если ещё не запущен)
# 2) собирает образы 4 сервисов ВНУТРИ minikube (реестр не нужен)
# 3) применяет манифесты по порядку: namespace → конфиг → инфраструктура → сервисы
# 4) ждёт готовности каждого Deployment и печатает адреса
#
# Запуск из корня репозитория:  ./k8s/deploy.sh
# Повторный запуск безопасен:   kubectl apply идемпотентен.
set -euo pipefail

cd "$(dirname "$0")/.." # корень репозитория — от него считаются пути

NS=rentcontrol
SERVICES=(Main.Api Payments.Api Main.Worker Realtime.Api)
TIMEOUT=600s

command -v minikube >/dev/null || { echo "✗ minikube не найден — см. k8s/README.md"; exit 1; }
command -v kubectl  >/dev/null || { echo "✗ kubectl не найден — см. k8s/README.md"; exit 1; }

# ── 1. Кластер ───────────────────────────────────────────────────────────────
# --container-runtime=docker: kubelet берёт образы из того же docker-демона,
# куда мы их собираем (с дефолтным containerd собранные образы не видны).
# --ports: публикует NodePort'ы из виртуалки на хост — сервисы доступны
# на localhost:30080 без туннелей и правок hosts (Docker Desktop не
# маршрутизирует IP виртуалки к хосту).
if ! minikube status >/dev/null 2>&1; then
  echo "── Запускаю кластер minikube (первый раз — несколько минут)..."
  minikube start --driver=docker --cpus=4 --memory=6g --container-runtime=docker \
    --ports=30080:30080 --ports=30081:30081 --ports=30082:30082 --ports=30672:30672
fi

# ── 2. Образы ────────────────────────────────────────────────────────────────
# Переключаем локальный docker CLI на демон ВНУТРИ виртуалки minikube:
# собранный образ сразу виден кластеру, пушить в реестр не нужно.
# Контекст сборки — каталог solution (COPY-пути в Dockerfile относительны него).
eval "$(minikube -p minikube docker-env --shell bash)"
for svc in "${SERVICES[@]}"; do
  tag="rentcontrol/$(echo "$svc" | tr '[:upper:]' '[:lower:]' | tr '.' '-')"
  echo "── Собираю образ $tag:local ..."
  docker build \
    -f "src/backend/RentControl/$svc/Dockerfile" \
    -t "$tag:local" \
    src/backend/RentControl
done

# ── 3. Namespace и конфигурация ──────────────────────────────────────────────
kubectl apply -f k8s/00-namespace.yaml
kubectl apply -f k8s/05-config.yaml

# ── 4. Инфраструктура: БД и брокер ───────────────────────────────────────────
echo "── Применяю postgres и rabbitmq..."
kubectl apply -f k8s/10-postgres.yaml
kubectl apply -f k8s/15-rabbitmq.yaml
kubectl -n "$NS" rollout status --timeout="$TIMEOUT" deployment/postgres
kubectl -n "$NS" rollout status --timeout="$TIMEOUT" deployment/rabbitmq

# ── 5. Сервисы ───────────────────────────────────────────────────────────────
echo "── Применяю сервисы..."
kubectl apply -f k8s/20-main-api.yaml
kubectl apply -f k8s/25-payments-api.yaml
kubectl apply -f k8s/30-main-worker.yaml
kubectl apply -f k8s/35-realtime-api.yaml
for d in main-api payments-api main-worker realtime-api; do
  kubectl -n "$NS" rollout status --timeout="$TIMEOUT" "deployment/$d"
done

# ── 6. Адреса ────────────────────────────────────────────────────────────────
echo
echo "════════════════ Готово ════════════════"
echo " Main.Api      http://localhost:30080   (/health, /swagger)"
echo " Payments.Api  http://localhost:30081"
echo " Realtime.Api  http://localhost:30082/weatherforecast"
echo " RabbitMQ UI   http://localhost:30672   (rentcontrol / rc-rabbit-pass)"
echo
echo " Поды:         kubectl -n ${NS} get pods"
echo " Логи:         kubectl -n ${NS} logs deploy/main-api -f"
echo " Снести всё:   ./k8s/teardown.sh"
