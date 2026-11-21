import { Field, Select } from "../../../shared/ui";
import type { BookingStatus } from "../../../types/booking";
import { bookingStatusLabels } from "../lib/status";

type BookingsFiltersProps = {
  status: BookingStatus | undefined;
  onStatusChange: (status: BookingStatus | undefined) => void;
};

export function BookingsFilters({
  status,
  onStatusChange,
}: BookingsFiltersProps) {
  return (
    <Field
      label="Статус"
      htmlFor="booking-status"
      className="max-w-xs"
    >
      <Select
        id="booking-status"
        value={status ?? ""}
        onChange={(event) => {
          const value = event.target.value;
          onStatusChange(value ? (Number(value) as BookingStatus) : undefined);
        }}
      >
        <option value="">Все</option>
        {Object.entries(bookingStatusLabels).map(([value, label]) => (
          <option key={value} value={value}>
            {label}
          </option>
        ))}
      </Select>
    </Field>
  );
}