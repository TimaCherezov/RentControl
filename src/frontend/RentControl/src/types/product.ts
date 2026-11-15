import type { ProductImageDto } from "./productImage";

export type ProductDto = {
  id: string;
  name: string;
  marketValue: number;
  depositOverride: number | null;
  depositAmount: number;
  dailyRate: number;
  hourlyRate: number;
  weeklyRate: number | null;
  monthlyRate: number | null;
  description: string | null;
  images: ProductImageDto[];
};

export type ProductMaxAvailableDuration = {
  maxDuration: string;
  totalSeconds: number;
};

export type CreateProductRequest = {
  name: string;
  marketValue: number;
  depositOverride?: number;
  dailyRate: number;
  hourlyRate: number;
  weeklyRate?: number;
  monthlyRate?: number;
  description?: string;
};

export type UpdateProductRequest = CreateProductRequest;

export type GetPageProductRequest = {
  pageNumber: number;
  pageSize: number;
};

export type GetMaxAvailableDuration = {
  startAt: string;
  locationId?: string;
};
