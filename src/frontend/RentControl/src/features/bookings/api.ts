import { httpGet, httpPost } from "../../lib/http";
import type {
  BookingDto,
  BookingQuote,
  CreateBookingRequest,
  GetPageBookingRequest,
} from "../../types/booking";
import type { PagedResponse } from "../../types/pagedResponse";

export const getBookings = (
  query?: GetPageBookingRequest,
  init?: RequestInit,
) => httpGet<PagedResponse<BookingDto>>("/api/bookings", { ...query }, init);

export const getBooking = (id: string, init?: RequestInit) =>
  httpGet<BookingDto>(`/api/bookings/${id}`, undefined, init);

export const createBooking = (data: CreateBookingRequest, init?: RequestInit) =>
  httpPost<BookingDto>("/api/bookings", data, init);

export const markAsCancelled = (id: string, init?: RequestInit) =>
  httpPost<void>(`/api/bookings/${id}/cancel`, undefined, init);

export const markAsReadyForPickup = (id: string, init?: RequestInit) =>
  httpPost<void>(`/api/bookings/${id}/ready-for-pickup`, undefined, init);

export const getBookingQuote = (
  productId: string,
  query: { startAt: string; endAt: string },
  init?: RequestInit,
) =>
  httpGet<BookingQuote>(
    `/api/products/${productId}/quote`,
    { ...query },
    init,
  );