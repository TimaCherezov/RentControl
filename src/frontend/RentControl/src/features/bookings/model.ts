import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type {
  CreateBookingRequest,
  GetPageBookingRequest,
} from "../../types/bookings";
import {
  createBooking,
  getBooking,
  getBookings,
  markAsCancelled,
  markAsReadyForPickup,
} from "./api";

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
  });

export const useCreateBooking = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateBookingRequest) => createBooking(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: BOOKINGS_KEY }),
  });
};

const makeStatusMutation = (
  id: string,
  markAs: (id: string) => Promise<unknown>,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => markAs(id),
    onSuccess: (_, id) => {
      qc.invalidateQueries({ queryKey: [...BOOKINGS_KEY, "detail", id] });
      qc.invalidateQueries({ queryKey: [...BOOKINGS_KEY] });
    },
  });
};

export const useMarkAsCancelled = (id: string) =>
  makeStatusMutation(id, markAsCancelled);

export const useMarkAsReadyForPickup = (id: string) =>
  makeStatusMutation(id, markAsReadyForPickup);
