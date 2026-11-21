#!/usr/bin/env bash
# Удаляет namespace rentcontrol вместе с данными (PVC). Кластер остаётся.
set -euo pipefail

kubectl delete namespace rentcontrol --ignore-not-found
