import {
  GetPageBookingRequest,
  BookingDto,
  CreateBookingRequest,
} from "../../types/bookings";
import { httpGet, httpPost } from "../../lib/http";
import { PagedResponse } from "../../types/pagedResponse";

export const getBookings = (
  query?: GetPageBookingRequest,
  init?: RequestInit,
) => httpGet<PagedResponse<BookingDto>>("/api/bookings", { ...query }, init);

export const getBooking = (id: string, init?: RequestInit) =>
  httpGet<BookingDto>(`/api/bookings/${id}`, undefined, init);

export const createBooking = (data: CreateBookingRequest, init?: RequestInit) =>
  httpPost("/api/bookings", data, init);

export const markAsCancelled = (id: string, init?: RequestInit) =>
  httpPost(`/api/bookings/${id}/cancel`, init);

export const markAsReadyForPickup = (id: string, init?: RequestInit) =>
  httpPost(`/api/bookings/${id}/ready-for-pickup`, init);
