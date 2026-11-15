import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import {
  Button,
  Container,
  EmptyState,
  ImageWithFallback,
  Skeleton,
  Stack,
} from "../shared/ui";
import { useBooking } from "../features/bookings/model";
import { BookingDetails } from "../features/bookings/ui/BookingDetails";
import { BookingStatusActions } from "../features/bookings/ui/BookingStatusActions";

export function BookingDetailPage() {
  const { id = "" } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const { data: booking, isLoading, error } = useBooking(id);
  const showBackToList =
    (location.state as { from?: string } | null)?.from === "bookings";

  if (isLoading) {
    return (
      <Container className="py-8">
        <Skeleton className="h-96 rounded-lg" />
      </Container>
    );
  }

  if (error || !booking) {
    return (
      <Container className="py-8">
        <EmptyState
          title="Бронь не найдена"
          description={
            error instanceof Error ? error.message : "Возможно, она удалена."
          }
          action={
            <Button variant="secondary" onClick={() => navigate("/bookings")}>
              К списку
            </Button>
          }
        />
      </Container>
    );
  }

  return (
    <Container className="py-8">
      <Stack gap="lg">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div className="flex items-center gap-3">
            {showBackToList && (
              <Button
                variant="primary"
                aria-label="К списку броней"
                className="size-10 px-0"
                onClick={() => navigate("/bookings")}
              >
                <svg
                  width="20"
                  height="20"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  aria-hidden
                >
                  <path d="M19 12H5" />
                  <path d="m12 19-7-7 7-7" />
                </svg>
              </Button>
            )}
            <h1 className="m-0">Бронь {booking.id.slice(0, 8)}</h1>
          </div>

          <BookingStatusActions booking={booking} />
        </div>

        <BookingDetails booking={booking} />

        {booking.productId && (
          <Link
            to={`/products/${booking.productId}`}
            className="flex w-full items-center gap-4 rounded-lg border border-border p-3 pr-4 text-start transition-colors hover:bg-accent-bg"
          >
            <div className="flex max-h-[120px] max-w-[120px] shrink-0 items-center justify-center overflow-hidden rounded-md bg-accent-bg">
              <ImageWithFallback
                src={booking.mainImage?.url}
                alt={booking.mainImage?.altText}
                name={booking.productName ?? "Продукт"}
                className="size-auto max-h-[120px] max-w-[120px] object-contain"
              />
            </div>
            <span className="font-medium text-text-h">
              {booking.productName ?? "Продукт"}
            </span>
          </Link>
        )}
      </Stack>
    </Container>
  );
}