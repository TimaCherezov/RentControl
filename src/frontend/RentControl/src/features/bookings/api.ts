import { GetPageBookingRequest, BookingDto } from "../../types/bookings";
import { httpGet } from "../../lib/http";
import { PagedResponse } from "../../types/pagedResponse";

export const getBookings = (init: RequestInit, query?: GetPageBookingRequest) =>
  httpGet<PagedResponse<BookingDto>>("/api/bookings", init, { ...query });

export const getBooking = (id: string, init: RequestInit) =>
  httpGet<BookingDto>(`/api/bookings/${id}`, init);
