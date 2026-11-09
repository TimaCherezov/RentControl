import { httpGet } from "../../lib/http";
import { GetPageProductRequest } from "../../types/products";

export const getProducts = (
  query?: GetPageProductRequest,
  init?: RequestInit,
) => httpGet("/api/products", { ...query }, init);

export const getProduct = (id: string, init?: RequestInit) =>
  httpGet(`/api/products/${id}`, undefined, init);
