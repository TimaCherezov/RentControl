import { useEffect } from "react";
import { Button, Stack } from "../../../shared/ui";
import { BookingStatus } from "../../../types/booking";
import type { BookingDto } from "../../../types/booking";
import {
  useMarkAsCancelled,
  useMarkAsReadyForPickup,
  usePayBooking,
} from "../model";

export function BookingStatusActions({ booking }: { booking: BookingDto }) {
  const cancel = useMarkAsCancelled(booking.id);
  const ready = useMarkAsReadyForPickup(booking.id);
  const pay = usePayBooking(booking.id);

  const { reset: resetCancel } = cancel;
  const { reset: resetReady } = ready;
  const { reset: resetPay } = pay;

  useEffect(() => {
    resetCancel();
    resetReady();
    resetPay();
    // Сбрасываем ошибки мутаций при смене статуса брони,
    // иначе старая ошибка одной кнопки висит над другой.
  }, [booking.bookingStatus, resetCancel, resetReady, resetPay]);

  const error = cancel.error ?? ready.error ?? pay.error;

  return (
    <Stack gap="sm" className="items-end">
      <Stack direction="row" gap="sm">
        {booking.bookingStatus === BookingStatus.Pending && (
          <Button
            variant="danger"
            isLoading={cancel.isPending}
            onClick={() => cancel.mutate()}
          >
            Отменить
          </Button>
        )}
        {booking.bookingStatus === BookingStatus.Pending &&
          booking.isInvoiceCreated && (
            <Button
              isLoading={pay.isPending}
              onClick={() => pay.mutate()}
            >
              Оплатить
            </Button>
          )}
        {booking.bookingStatus === BookingStatus.Confirmed && (
          <Button
            isLoading={ready.isPending}
            onClick={() => ready.mutate()}
          >
            Я на пункте
          </Button>
        )}
      </Stack>
      {booking.bookingStatus === BookingStatus.Confirmed && (
        <p className="text-right text-xs text-text">
          Нажмите, когда приедете на пункт выдачи
        </p>
      )}
      {error && (
        <p role="alert" className="text-right text-sm text-danger">
          {error instanceof Error ? error.message : "Не удалось обновить статус"}
        </p>
      )}
    </Stack>
  );
}