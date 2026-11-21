import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import type {
  CreateBookingRequest,
  GetPageBookingRequest,
} from "../../types/booking";
import {
  createBooking,
  getBooking,
  getBookingQuote,
  getBookings,
  markAsCancelled,
  markAsReadyForPickup,
} from "./api";
import { fakePay, payByBooking } from "../payments/api";

const BOOKINGS_KEY = ["bookings"] as const;

export const useBookings = (query?: GetPageBookingRequest) =>
  useQuery({
    queryKey: [...BOOKINGS_KEY, "list", query ? { ...query } : undefined],
    queryFn: () => getBookings(query),
  });

export const useBooking = (id: string) =>
  useQuery({
    queryKey: [...BOOKINGS_KEY, "detail", id],
    queryFn: () => getBooking(id),
    enabled: Boolean(id),
  });

export const useCreateBooking = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateBookingRequest) => createBooking(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: BOOKINGS_KEY }),
  });
};

const useStatusMutation = (
  id: string,
  markAs: (id: string) => Promise<void>,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => markAs(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [...BOOKINGS_KEY, "detail", id] });
      qc.invalidateQueries({ queryKey: BOOKINGS_KEY });
    },
  });
};

export const useMarkAsCancelled = (id: string) =>
  useStatusMutation(id, markAsCancelled);

export const useMarkAsReadyForPickup = (id: string) =>
  useStatusMutation(id, markAsReadyForPickup);

export const usePayBooking = (bookingId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async () => {
      const { paymentUrl } = await payByBooking(bookingId);
      const invoiceId = paymentUrl.split("/").filter(Boolean).pop();

      if (!invoiceId) {
        throw new Error("Не удалось получить счёт");
      }

      return fakePay(invoiceId);
    },
    onSuccess: async () => {
      await new Promise((resolve) => setTimeout(resolve, 1_500));
      qc.invalidateQueries({ queryKey: BOOKINGS_KEY });
    },
  });
};

export const useBookingQuote = (
  productId: string,
  startAt: string,
  endAt: string,
) =>
  useQuery({
    queryKey: [...BOOKINGS_KEY, "quote", productId, startAt, endAt],
    queryFn: () => getBookingQuote(productId, { startAt, endAt }),
    enabled: Boolean(productId && startAt && endAt),
    placeholderData: keepPreviousData,
  });