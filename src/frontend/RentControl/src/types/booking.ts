import type { ProductImageDto } from "./productImage";

export type BookingDto = {
  id: string;
  locationName: string | null;
  locationAddress: string | null;
  startAt: string;
  endAt: string;
  depositAmount: number;
  bookingStatus: BookingStatus;
  statusChangedAt: string;
  productName: string | null;
  productId: string | null;
  mainImage: ProductImageDto | null;
  actualReturnAt: string | null;
  totalRentalCost: number | null;
  isInvoiceCreated: boolean;
};

export type CreateBookingRequest = {
  productId: string;
  startAt: string;
  endAt: string;
  locationId?: string;
};

export type UpdateBookingRequest = {
  productUnitId: string;
  locationId: string;
  startAt: string;
  endAt: string;
  actualReturnAt?: string;
  depositAmount: number;
  totalRentalCost?: number;
  bookingStatus: BookingStatus;
  statusChangedAt: string;
};

export type GetPageBookingRequest = {
  pageNumber: number;
  pageSize: number;
  status?: BookingStatus;
};

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

export type BookingQuoteLine = {
  unit: "hour" | "day" | "week" | "month";
  count: number;
  rate: number;
  subtotal: number;
};

export type BookingQuote = {
  productId: string;
  startAt: string;
  endAt: string;
  durationSeconds: number;
  rentalCost: number;
  depositAmount: number;
  totalAmount: number;
  lines: BookingQuoteLine[];
};
