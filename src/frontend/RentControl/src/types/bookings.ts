import { Temporal } from "@js-temporal/polyfill";
import Big from "big.js";

export class BookingDto {
  readonly id: string;
  readonly userId: string;
  readonly productUnitId: string;
  readonly locationId: string;
  readonly startAt: Temporal.ZonedDateTime;
  readonly endAt: Temporal.ZonedDateTime;
  readonly actualReturnAt?: Temporal.ZonedDateTime;
  readonly depositAmount: Big;
  readonly totalRentalCost?: Big;
  readonly bookingStatus: BookingStatus;
  readonly statusChangedAt: Temporal.ZonedDateTime;
  readonly isInvoiceCreated: boolean;
}

export class CreateBookingRequest {}

export class UpdateBookingRequest {}

export class GetPageBookingRequest {
  pageNumber: number;
  pageSize: number;
  userId?: string;
  status?: BookingStatus;
}

export const BookingStatus = {
  Pending: 1,
  Cancelled: 2,
  Confirmed: 3,
  ReadyForPickup: 4,
  Active: 5,
  Overdue: 6,
  Returned: 7,
  Completed: 8,
} as const;

export type BookingStatus = (typeof BookingStatus)[keyof typeof BookingStatus];
