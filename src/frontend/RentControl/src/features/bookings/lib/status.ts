import type { BookingStatus } from "../../../types/booking";

export const bookingStatusLabels: Record<BookingStatus, string> = {
  1: "Ожидает",
  2: "Отменено",
  3: "Подтверждено",
  4: "Ожидает выдачи",
  5: "Активно",
  6: "Просрочено",
  7: "Возвращено",
  8: "Завершено",
};

export const bookingStatusTones: Record<
  BookingStatus,
  "neutral" | "accent" | "success" | "warning" | "danger"
> = {
  1: "warning",
  2: "danger",
  3: "accent",
  4: "accent",
  5: "success",
  6: "danger",
  7: "neutral",
  8: "success",
};