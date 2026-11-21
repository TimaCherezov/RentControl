# Локальный деплой RentControl в minikube

Этот каталог превращает стек RentControl из «Aspire поднимает процессы на моей машине» в
«Kubernetes поднимает контейнеры в кластере» — как это будет в проде, только кластер
локальный (одна виртуалка на твоём компьютере).

## Что такое minikube и зачем

**Kubernetes (k8s)** — оркестратор контейнеров: сам запускает их на нужных машинах,
перезапускает упавших, распределяет трафик между копиями, раздаёт конфигурацию и секреты.

**minikube** — настоящий k8s-кластер на одной виртуалке. Устанавливается за минуту,
используется везде, чтобы учить k8s и гонять интеграцию локально.

## Пререквизиты

| Инструмент | Зачем | Как поставить |
|---|---|---|
| Docker Desktop | minikube поднимает виртуалку в docker | [docker.com](https://www.docker.com/products/docker-desktop/) |
| minikube | сам кластер | `winget install Kubernetes.minikube` |
| kubectl | CLI для команд кластеру | `winget install Kubernetes.kubectl` (или `minikube kubectl`) |

После установки **открой новый терминал**, чтобы обновился PATH.

## Быстрый старт

```bash
./k8s/deploy.sh        # сборка образов + деплой всего стека (первый раз ~15–20 минут)
```

Скрипт: поднимает кластер → собирает 4 образа внутри minikube → применяет манифесты
по порядку (конфиг → postgres/rabbitmq → сервисы) → ждёт готовности → печатает адреса.

| Что | Адрес |
|---|---|
| Main.Api | `http://localhost:30080` |
| Payments.Api | `http://localhost:30081` |
| Realtime.Api | `http://localhost:30082/weatherforecast` |
| RabbitMQ UI | `http://localhost:30672` — логин `rentcontrol` / `rc-rabbit-pass` |

Адреса — это NodePort'ы, опубликованные из виртуалки на хост флагом `--ports`
в deploy.sh: Docker Desktop не маршрутизирует IP виртуалки (`minikube ip`) к хосту,
поэтому сервисы открываются именно через localhost.

## Словарик: что лежит в k8s/*.yaml

| Объект | Что делает у нас |
|---|---|
| **Namespace** `rentcontrol` | «папка» кластера со всем стеком; удаляется одной командой |
| **ConfigMap** `app-config` | env-переменные без секретов (`ASPNETCORE_ENVIRONMENT`, `ServiceDiscovery__MainApi`, …). Ключи с `__` превращаются в `Раздел:Подраздел` конфига .NET |
| **Secret** `app-secrets` | пароли и строки подключения (`ConnectionStrings__maindb`, …) |
| **Deployment** | менеджер подов: держит 1 реплику, перезапускает упавших, катит обновления |
| **Pod** | контейнер сервиса (postgres, rabbitmq, main-api, payments-api, main-worker, realtime-api) |
| **Service** | стабильное DNS-имя группы подов: `http://main-api`, `Host=postgres`, `amqp://rabbitmq` |
| **NodePort** | тип Service: открывает порт наружу виртуалки (30080–30672) — то, что видно в браузере |
| **PVC** | «заявка на диск»: данные postgres/rabbitmq и картинки товаров переживают перезапуск подов |
| **Probe** | k8s ходит на `/health` (готов принимать трафик?) и `/alive` (живой?), при проблемах убирает трафик/перезапускает |

Имена сервисов (`main-api`, `postgres`, `rabbitmq`) совпадают с тем, что Aspire даёт локально,
поэтому конфиги кода не менялись — только переменные окружения.

## Зачем подам `ASPNETCORE_ENVIRONMENT=Development`

Миграции БД при старте (`Main.Api/Program.cs`) и health-эндпоинты (`ServiceDefaults/Extensions.cs`)
сейчас включаются только в Development. В локальном кластере это удобно (ноль изменений в коде),
но **так не делают в проде**: там миграции выносят в отдельные Job'ы, а health-эндпоинты
открывают осознанно. Это следующий шаг, когда дойдёте до прод-деплоя.

## Шпаргалка kubectl

```bash
kubectl -n rentcontrol get pods                 # что запущено и в каком состоянии
kubectl -n rentcontrol get all                  # поды + сервисы + deployment'ы
kubectl -n rentcontrol logs deploy/main-api -f  # логи сервиса (stream)
kubectl -n rentcontrol logs deploy/payments-api --tail=100
kubectl -n rentcontrol describe pod -l app=main-api   # почему под не стартует
kubectl -n rentcontrol exec deploy/postgres -- psql -U postgres -d maindb  # SQL-консоль
kubectl -n rentcontrol get events --sort-by=.lastTimestamp  # хроника событий
```

Состояние пода = его жизнь: `Running 1/1` — ок; `CrashLoopBackOff` — падает при старте
(смотри `logs`); `ImagePullBackOff` — образ не найден (пересобери: `deploy.sh`); `Pending` —
не хватает ресурсов.

## Пересобрать один сервис после правок кода

```bash
eval $(minikube docker-env)   # направить docker CLI на демон внутри minikube
docker build -f src/backend/RentControl/Main.Api/Dockerfile \
  -t rentcontrol/main-api:local src/backend/RentControl
kubectl -n rentcontrol rollout restart deployment/main-api
```

`eval $(minikube docker-env)` настраивает переменные окружения, и `docker build` кладёт
образ прямо в демон виртуалки minikube — кластер видит его сразу, реестр не нужен.
Кластер запускается с `--container-runtime=docker` именно поэтому: kubelet берёт образы
из того же демона. `rollout restart` перезапускает поды с новым образом.

## Проверить бизнес-флоу сквозь все сервисы

Регистрация → бронирование → инвойс → оплата — события идут
`main-api → (RabbitMQ) → payments-api → (RabbitMQ) → main-worker → БД`:

```bash
# 1. регистрация и логин
curl -s -X POST http://localhost:30080/api/auth/register -H 'Content-Type: application/json' \
  -d '{"name":"Me","email":"me@test.local","password":"Passw0rd!"}'
TOKEN=$(curl -s -X POST http://localhost:30080/api/auth/login -H 'Content-Type: application/json' \
  -d '{"email":"me@test.local","password":"Passw0rd!"}' | <достать accessToken>)
# 2. создать location → product → product-unit → booking (эндпоинты Main.Api)
# 3. в payments-api появляется инвойс:
curl -s http://localhost:30081/api/invoices/fake-pay/<invoiceId>   # оплата-заглушка
# 4. статус бронирования изменился — смотреть в БД:
kubectl -n rentcontrol exec deploy/postgres -- \
  psql -U postgres -d maindb -c 'select id, booking_status from bookings'
```

В RabbitMQ UI (`http://localhost:30672`) на вкладке Exchanges видны
`OrderCreated`, `InvoiceCreated`, `PaymentSucceeded`, а в Queues — очереди
`order-created`, `invoice-created`, `payment-succeeded`.

## Снести

```bash
./k8s/teardown.sh      # удаляет namespace со всем стеком И данными (PVC)
minikube delete        # (опционально) снести и сам кластер
```

## Известные ограничения локальной схемы

- 1 реплика каждого сервиса — single-инстанс postgres/rabbitmq без отказоустойчивости;
- Development-режим в подах (см. выше) — миграции «изнутри» сервиса;
- пароли-заглушки в Secret закоммичены в репозиторий — для локала ок, в проде так нельзя;
- NodePort'ы опубликованы в deploy.sh фиксированными флагами `--ports`; если добавишь
  новый NodePort в манифестах — добавь и `--ports`, иначе хост его не увидит.
