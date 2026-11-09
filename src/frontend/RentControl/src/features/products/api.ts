import { httpGet } from "../../lib/http";
import { GetPageProductRequest } from "../../types/products";

export const getProducts = (init: RequestInit, query?: GetPageProductRequest) =>
  httpGet("/api/products", init, { ...query });

export const getProduct = (id: string, init: RequestInit) =>
  httpGet(`/api/products/${id}`, init);
