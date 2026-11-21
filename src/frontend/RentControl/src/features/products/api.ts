import { httpGet } from "../../lib/http";
import type {
  GetMaxAvailableDuration,
  GetPageProductRequest,
  ProductDto,
  ProductMaxAvailableDuration,
} from "../../types/product";
import type { PagedResponse } from "../../types/pagedResponse";

export const getProducts = (
  query?: GetPageProductRequest,
  init?: RequestInit,
) => httpGet<PagedResponse<ProductDto>>("/api/products", { ...query }, init);

export const getProduct = (id: string, init?: RequestInit) =>
  httpGet<ProductDto>(`/api/products/${id}`, undefined, init);

export const getMaxAvailableDuration = (
  id: string,
  query: GetMaxAvailableDuration,
  init?: RequestInit,
) =>
  httpGet<ProductMaxAvailableDuration>(
    `/api/products/${id}/max-duration`,
    { ...query },
    init,
  );