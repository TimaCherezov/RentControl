import { useState, type FormEvent } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import {
  Button,
  Card,
  EmptyState,
  Field,
  ImageWithFallback,
  Input,
  Skeleton,
  Spinner,
  Stack,
} from "../../../shared/ui";
import {
  formatCurrency,
  formatMaxDuration,
  fromDateTimeLocalValue,
  toDateTimeLocalValue,
} from "../../../shared/utils/format";
import { useDebouncedValue } from "../../../shared/utils/useDebouncedValue";
import { useAuth } from "../../../app/auth/useAuth";
import type { CreateBookingRequest } from "../../../types/booking";
import {
  useMaxAvailableDuration,
  useProduct,
} from "../../products/models";
import { useBookingQuote, useCreateBooking } from "../model";
import { DurationPicker } from "./DurationPicker";

const HOUR = 3_600;

const UNIT_LABELS: Record<"hour" | "day" | "week" | "month", string> = {
  hour: "час",
  day: "день",
  week: "неделя",
  month: "месяц",
};

type FormErrors = Partial<Record<"startAt" | "duration", string>>;

export function CreateBookingForm() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const productId = searchParams.get("productId") ?? "";

  const [startAt, setStartAt] = useState(() =>
    toDateTimeLocalValue(new Date(Date.now() + HOUR * 1_000)),
  );
  const [durationSeconds, setDurationSeconds] = useState(2 * HOUR);
  const [errors, setErrors] = useState<FormErrors>({});

  const productQuery = useProduct(productId);
  const product = productQuery.data;

  const startAtDate = startAt ? new Date(startAt) : null;
  const startAtIso =
    startAtDate && !Number.isNaN(startAtDate.getTime())
      ? fromDateTimeLocalValue(startAt)
      : "";

  const debouncedStart = useDebouncedValue(startAtIso, 300);
  const maxDuration = useMaxAvailableDuration(productId, debouncedStart);
  const maxSeconds = maxDuration.data?.totalSeconds ?? 0;
  const effectiveDurationSeconds = Math.min(durationSeconds, maxSeconds);

  const debouncedDuration = useDebouncedValue(effectiveDurationSeconds, 300);
  const debouncedEnd =
    debouncedStart && debouncedDuration > 0
      ? new Date(
          new Date(debouncedStart).getTime() + debouncedDuration * 1_000,
        ).toISOString()
      : "";

  const endAtIso =
    startAtIso && effectiveDurationSeconds > 0
      ? new Date(
          new Date(startAtIso).getTime() + effectiveDurationSeconds * 1_000,
        ).toISOString()
      : "";

  const quote = useBookingQuote(productId, debouncedStart, debouncedEnd);
  const create = useCreateBooking();

  const canSubmit = Boolean(
    product &&
      startAtIso &&
      endAtIso &&
      effectiveDurationSeconds > 0 &&
      maxSeconds > 0 &&
      effectiveDurationSeconds <= maxSeconds,
  );

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const nextErrors: FormErrors = {};

    if (!startAtIso) nextErrors.startAt = "Укажите дату начала";
    if (startAtIso && new Date(startAtIso).getTime() <= Date.now()) {
      nextErrors.startAt = "Начало не может быть в прошлом";
    }
    if (effectiveDurationSeconds <= 0) {
      nextErrors.duration = "Выберите длительность";
    }
    if (maxSeconds > 0 && effectiveDurationSeconds > maxSeconds) {
      nextErrors.duration = `Максимальный срок: ${formatMaxDuration(maxSeconds)}`;
    }

    setErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) return;
    if (!user || !product) return;

    const payload: CreateBookingRequest = {
      productId,
      startAt: startAtIso,
      endAt: endAtIso,
    };

    create.mutate(payload, {
      onSuccess: (booking) => navigate(`/bookings/${booking.id}`),
    });
  };

  if (!productId) {
    return (
      <EmptyState
        title="Продукт не выбран"
        description="Откройте страницу продукта и нажмите «Забронировать»."
        action={
          <Link to="/products">
            <Button variant="secondary">К продуктам</Button>
          </Link>
        }
      />
    );
  }

  if (productQuery.isLoading) {
    return <Skeleton className="h-64 rounded-lg" />;
  }

  if (productQuery.error || !product) {
    return (
      <EmptyState
        title="Продукт не найден"
        description={
          productQuery.error instanceof Error
            ? productQuery.error.message
            : "Возможно, он удалён."
        }
        action={
          <Link to="/products">
            <Button variant="secondary">К продуктам</Button>
          </Link>
        }
      />
    );
  }

  const image =
    product.images.find((item) => item.isMain) ?? product.images[0];

  return (
    <div className="grid gap-6 lg:grid-cols-2 lg:items-start">
      <div className="lg:sticky lg:top-8">
        <Card padding="none" className="overflow-hidden">
          <div className="aspect-[4/3] w-full bg-accent-bg">
            <ImageWithFallback
              src={image?.url}
              alt={image?.altText}
              name={product.name}
            />
          </div>
          <div className="flex flex-col gap-2 p-4 text-start">
            <h2 className="text-xl">{product.name}</h2>
            <dl className="grid grid-cols-2 gap-x-4 gap-y-1 text-sm">
              <div className="flex items-center justify-between gap-2">
                <dt className="text-text">В час</dt>
                <dd className="font-medium text-text-h">
                  {formatCurrency(product.hourlyRate)}
                </dd>
              </div>
              <div className="flex items-center justify-between gap-2">
                <dt className="text-text">В день</dt>
                <dd className="font-medium text-text-h">
                  {formatCurrency(product.dailyRate)}
                </dd>
              </div>
              {typeof product.weeklyRate === "number" && (
                <div className="flex items-center justify-between gap-2">
                  <dt className="text-text">В неделю</dt>
                  <dd className="font-medium text-text-h">
                    {formatCurrency(product.weeklyRate)}
                  </dd>
                </div>
              )}
              {typeof product.monthlyRate === "number" && (
                <div className="flex items-center justify-between gap-2">
                  <dt className="text-text">В месяц</dt>
                  <dd className="font-medium text-text-h">
                    {formatCurrency(product.monthlyRate)}
                  </dd>
                </div>
              )}
              <div className="col-span-2 flex items-center justify-between gap-2">
                <dt className="text-text">Депозит</dt>
                <dd className="font-medium text-text-h">
                  {formatCurrency(product.depositAmount)}
                </dd>
              </div>
            </dl>
          </div>
        </Card>
      </div>

      <form onSubmit={handleSubmit} noValidate>
        <Stack gap="md">
          <Field
            label="Начало"
            htmlFor="start-at"
            required
            error={errors.startAt}
          >
            <Input
              id="start-at"
              type="datetime-local"
              value={startAt}
              invalid={Boolean(errors.startAt)}
              onChange={(event) => setStartAt(event.target.value)}
            />
          </Field>

          <div className="flex flex-col gap-1.5">
            <span className="text-sm font-medium text-text-h">
              Длительность
            </span>

            {!startAtIso ? (
              <p className="text-sm text-text">Укажите дату начала.</p>
            ) : maxDuration.isLoading ? (
              <Spinner size="sm" label="Проверка доступности" />
            ) : (
              <DurationPicker
                valueSeconds={effectiveDurationSeconds}
                maxSeconds={maxSeconds}
                onChange={setDurationSeconds}
                disabled={create.isPending}
              />
            )}

            {errors.duration && (
              <p role="alert" className="text-xs text-danger">
                {errors.duration}
              </p>
            )}

            {maxDuration.isError && (
              <p className="text-xs text-danger">
                {maxDuration.error instanceof Error
                  ? maxDuration.error.message
                  : "Не удалось получить доступный срок"}
              </p>
            )}
          </div>

          {(quote.data || quote.isFetching || quote.error) && (
            <div className="rounded-md border border-border p-3">
              {quote.isFetching && !quote.data ? (
                <Spinner size="sm" label="Расчёт стоимости" />
              ) : null}

              {quote.data ? (
                <Stack gap="sm">
                  <ul className="flex flex-col gap-1 text-sm text-text">
                    {quote.data.lines.map((line) => (
                      <li
                        key={line.unit}
                        className="flex justify-between gap-3"
                      >
                        <span>
                          {line.count} × {UNIT_LABELS[line.unit]}
                        </span>
                        <span>{formatCurrency(line.subtotal)}</span>
                      </li>
                    ))}
                  </ul>

                  <dl className="grid grid-cols-3 gap-2 text-sm">
                    <div>
                      <dt className="text-xs text-text">Аренда</dt>
                      <dd className="font-medium text-text-h">
                        {formatCurrency(quote.data.rentalCost)}
                      </dd>
                    </div>
                    <div>
                      <dt className="text-xs text-text">
                        Депозит (возвратный)
                      </dt>
                      <dd className="font-medium text-text-h">
                        {formatCurrency(quote.data.depositAmount)}
                      </dd>
                    </div>
                    <div>
                      <dt className="text-xs text-text">
                        Итого с депозитом
                      </dt>
                      <dd className="font-medium text-text-h">
                        {formatCurrency(quote.data.totalAmount)}
                      </dd>
                    </div>
                  </dl>
                </Stack>
              ) : null}

              {quote.error ? (
                <p className="text-xs text-danger">
                  {quote.error instanceof Error
                    ? quote.error.message
                    : "Не удалось рассчитать стоимость"}
                </p>
              ) : null}
            </div>
          )}

          {create.error && (
            <p role="alert" className="text-sm text-danger">
              {create.error instanceof Error
                ? create.error.message
                : "Не удалось создать бронь"}
            </p>
          )}

          <div>
            <Button
              type="submit"
              isLoading={create.isPending}
              disabled={!canSubmit}
            >
              Создать бронь
            </Button>
          </div>
        </Stack>
      </form>
    </div>
  );
}