import { EmptyState, Skeleton } from "../../../shared/ui";
import type { BookingDto } from "../../../types/booking";
import { BookingCard } from "./BookingCard";

type BookingListProps = {
  bookings: BookingDto[];
  isLoading?: boolean;
  error?: unknown;
};

export function BookingList({ bookings, isLoading, error }: BookingListProps) {
  if (isLoading) {
    return (
      <div className="flex flex-col gap-3">
        {Array.from({ length: 5 }).map((_, index) => (
          <Skeleton key={index} className="h-32 rounded-lg" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <EmptyState
        title="Не удалось загрузить брони"
        description={
          error instanceof Error ? error.message : "Попробуйте позже"
        }
      />
    );
  }

  if (bookings.length === 0) {
    return (
      <EmptyState
        title="Брони не найдены"
        description="Пока нет броней."
      />
    );
  }

  return (
    <div className="flex flex-col gap-3">
      {bookings.map((booking) => (
        <BookingCard key={booking.id} booking={booking} />
      ))}
    </div>
  );
}