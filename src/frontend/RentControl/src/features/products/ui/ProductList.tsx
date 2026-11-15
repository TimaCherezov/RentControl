import { EmptyState, Skeleton } from "../../../shared/ui";
import type { ProductDto } from "../../../types/product";
import { ProductCard } from "./ProductCard";

type ProductListProps = {
  products: ProductDto[];
  isLoading?: boolean;
  error?: unknown;
};

export function ProductList({ products, isLoading, error }: ProductListProps) {
  if (isLoading) {
    return (
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <Skeleton key={index} className="h-72 rounded-lg" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <EmptyState
        title="Не удалось загрузить продукты"
        description={
          error instanceof Error ? error.message : "Попробуйте позже"
        }
      />
    );
  }

  if (products.length === 0) {
    return (
      <EmptyState
        title="Продукты не найдены"
        description="Пока нет доступных продуктов."
      />
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
}