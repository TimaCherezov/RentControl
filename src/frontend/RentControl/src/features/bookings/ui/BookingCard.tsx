import { Link } from "react-router-dom";
import { Card, ImageWithFallback } from "../../../shared/ui";
import { formatCurrency, formatDateTime } from "../../../shared/utils/format";
import type { BookingDto } from "../../../types/booking";
import { BookingStatusBadge } from "./BookingStatusBadge";

export function BookingCard({ booking }: { booking: BookingDto }) {
  return (
    <Link
      to={`/bookings/${booking.id}`}
      state={{ from: "bookings" }}
      className="block rounded-lg focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent"
    >
      <Card className="flex gap-4 text-start transition-shadow hover:shadow-lg">
        <div className="size-24 shrink-0 overflow-hidden rounded-md bg-accent-bg">
          <ImageWithFallback
            src={booking.mainImage?.url}
            alt={booking.mainImage?.altText}
            name={booking.productName ?? "Продукт"}
          />
        </div>
        <div className="flex min-w-0 flex-1 flex-col gap-1">
          <div className="flex items-center justify-between gap-2">
            <h3 className="truncate font-medium text-text-h">
              {booking.productName ?? "Продукт"}
            </h3>
            <BookingStatusBadge status={booking.bookingStatus} />
          </div>
          <p className="text-sm text-text">
            {formatDateTime(booking.startAt)} — {formatDateTime(booking.endAt)}
          </p>
          <p className="text-sm text-text">
            {booking.locationName ?? "Локация не указана"}
          </p>
          <p className="text-sm font-medium text-text-h">
            {booking.totalRentalCost !== null
              ? formatCurrency(
                  booking.totalRentalCost + booking.depositAmount,
                )
              : `Депозит: ${formatCurrency(booking.depositAmount)}`}
          </p>
        </div>
      </Card>
    </Link>
  );
}