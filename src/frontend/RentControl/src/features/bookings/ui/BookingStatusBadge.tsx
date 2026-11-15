import { Badge } from "../../../shared/ui";
import type { BookingStatus } from "../../../types/booking";
import { bookingStatusLabels, bookingStatusTones } from "../lib/status";

export function BookingStatusBadge({ status }: { status: BookingStatus }) {
  return (
    <Badge tone={bookingStatusTones[status]}>
      {bookingStatusLabels[status]}
    </Badge>
  );
}