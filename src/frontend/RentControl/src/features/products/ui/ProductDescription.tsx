import { Card } from "../../../shared/ui";
import type { ProductDto } from "../../../types/product";

export function ProductDescription({ product }: { product: ProductDto }) {
  if (!product.description) return null;

  return (
    <Card className="flex flex-col gap-2 text-start">
      <h2>Описание</h2>
      <p className="whitespace-pre-line text-text">{product.description}</p>
    </Card>
  );
}