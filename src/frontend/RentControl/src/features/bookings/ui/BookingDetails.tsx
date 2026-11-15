import type { ReactNode } from "react";
import { Card } from "../../../shared/ui";
import { formatCurrency, formatDateTime } from "../../../shared/utils/format";
import type { BookingDto } from "../../../types/booking";
import { BookingStatusBadge } from "./BookingStatusBadge";

export function BookingDetails({ booking }: { booking: BookingDto }) {
  const rows: Array<{ label: string; value: ReactNode }> = [
    {
      label: "Статус",
      value: <BookingStatusBadge status={booking.bookingStatus} />,
    },
    { label: "Продукт", value: booking.productName ?? "—" },
    { label: "Локация", value: booking.locationName ?? "—" },
    { label: "Адрес", value: booking.locationAddress ?? "—" },
    { label: "Начало", value: formatDateTime(booking.startAt) },
    { label: "Окончание", value: formatDateTime(booking.endAt) },
    {
      label: "Возврат",
      value: booking.actualReturnAt
        ? formatDateTime(booking.actualReturnAt)
        : "—",
    },
    { label: "Депозит", value: formatCurrency(booking.depositAmount) },
    {
      label: "Итого",
      value:
        booking.totalRentalCost !== null
          ? formatCurrency(
              booking.totalRentalCost + booking.depositAmount,
            )
          : "—",
    },
    { label: "Счёт создан", value: booking.isInvoiceCreated ? "Да" : "Нет" },
    {
      label: "Статус изменён",
      value: formatDateTime(booking.statusChangedAt),
    },
  ];

  return (
    <Card>
      <dl className="grid gap-3 sm:grid-cols-2">
        {rows.map((row) => (
          <div key={row.label} className="flex flex-col gap-0.5">
            <dt className="text-xs text-text">{row.label}</dt>
            <dd className="text-sm text-text-h">{row.value}</dd>
          </div>
        ))}
      </dl>
    </Card>
  );
}