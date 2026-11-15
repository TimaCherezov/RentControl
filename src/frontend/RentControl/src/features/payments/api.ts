import { httpGet, httpPost } from "../../lib/http";

export const payByBooking = (bookingId: string) =>
  httpPost<{ paymentUrl: string }>(
    `/api/invoices/by-booking/${bookingId}/pay`,
  );

export const fakePay = (invoiceId: string) =>
  httpGet<{ status: string; bookingId: string }>(
    `/api/invoices/fake-pay/${invoiceId}`,
  );