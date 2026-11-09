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

export class CreateBookingRequest {
  readonly productUnitId: string;
  readonly startAt: Temporal.ZonedDateTime;
  readonly endAt: Temporal.ZonedDateTime;
  readonly locationId: string;
  readonly actualReturnAt?: string;
  readonly totalRentalCost?: Big;
  readonly depositAmount: Big;
  readonly statusChangedAt: Temporal.ZonedDateTime;
  readonly bookingStatus: BookingStatus;
  readonly userId: string;

  constructor(
    productUnitId: string,
    startAt: Temporal.ZonedDateTime,
    endAt: Temporal.ZonedDateTime,
    locationId: string,
    depositAmount: Big,
    statusChangedAt: Temporal.ZonedDateTime,
    bookingStatus: BookingStatus,
    userId: string,
    actualReturnAt?: string,
    totalRentalCost?: Big,
  ) {
    this.productUnitId = productUnitId;
    this.startAt = startAt;
    this.endAt = endAt;
    this.locationId = locationId;
    this.actualReturnAt = actualReturnAt;
    this.totalRentalCost = totalRentalCost;
    this.depositAmount = depositAmount;
    this.statusChangedAt = statusChangedAt;
    this.bookingStatus = bookingStatus;
    this.userId = userId;
  }
}

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
