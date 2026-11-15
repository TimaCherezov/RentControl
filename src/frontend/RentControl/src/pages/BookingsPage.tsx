import { useSearchParams } from "react-router-dom";
import { Container, Pagination, Stack } from "../shared/ui";
import { useBookings } from "../features/bookings/model";
import { BookingList } from "../features/bookings/ui/BookingList";
import { BookingsFilters } from "../features/bookings/ui/BookingsFilters";
import type { BookingStatus } from "../types/booking";

const PAGE_SIZE = 10;

function parseStatus(value: string | null): BookingStatus | undefined {
  if (!value) return undefined;
  const parsed = Number(value);
  return Number.isInteger(parsed) && parsed >= 1 && parsed <= 8
    ? (parsed as BookingStatus)
    : undefined;
}

export function BookingsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = Math.max(1, Number(searchParams.get("page") ?? 1));
  const status = parseStatus(searchParams.get("status"));
  const { data, isLoading, error } = useBookings({
    pageNumber: page,
    pageSize: PAGE_SIZE,
    status,
  });

  const setPage = (nextPage: number) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      if (nextPage <= 1) next.delete("page");
      else next.set("page", String(nextPage));
      return next;
    });
  };

  const setStatus = (nextStatus: BookingStatus | undefined) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      next.delete("page");
      if (nextStatus === undefined) next.delete("status");
      else next.set("status", String(nextStatus));
      return next;
    });
  };

  return (
    <Container className="py-8">
      <Stack gap="lg">
        <h1>Брони</h1>
        <BookingsFilters status={status} onStatusChange={setStatus} />
        <BookingList
          bookings={data?.items ?? []}
          isLoading={isLoading}
          error={error}
        />
        {data && data.totalCount > PAGE_SIZE && (
          <Pagination
            page={data.pageNumber}
            pageSize={data.pageSize}
            totalCount={data.totalCount}
            onPageChange={setPage}
          />
        )}
      </Stack>
    </Container>
  );
}