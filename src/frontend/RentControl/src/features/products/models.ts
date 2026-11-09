import { useQuery } from "@tanstack/react-query";
import type { GetPageProductRequest } from "../../types/products";
import { getProduct, getProducts } from "./api";

const PRODUCTS_KEY = ["products"] as const;

export const useProducts = (query?: GetPageProductRequest) =>
  useQuery({
    queryKey: [...PRODUCTS_KEY, "list", query ? { ...query } : undefined],
    queryFn: () => getProducts(query),
  });

export const useProduct = (id: string) =>
  useQuery({
    queryKey: [...PRODUCTS_KEY, "detail", id],
    queryFn: () => getProduct(id),
  });
