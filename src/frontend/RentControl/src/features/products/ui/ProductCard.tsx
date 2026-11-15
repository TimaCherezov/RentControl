import { Link } from "react-router-dom";
import { Card, ImageWithFallback } from "../../../shared/ui";
import { formatCurrency } from "../../../shared/utils/format";
import type { ProductDto } from "../../../types/product";

export function ProductCard({ product }: { product: ProductDto }) {
  const image = product.images.find((item) => item.isMain) ?? product.images[0];

  return (
    <Link
      to={`/products/${product.id}`}
      className="block rounded-lg focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent"
    >
      <Card
        padding="none"
        className="h-full overflow-hidden transition-shadow hover:shadow-lg"
      >
        <div className="aspect-[4/3] w-full bg-accent-bg">
          <ImageWithFallback
            src={image?.url}
            alt={image?.altText}
            name={product.name}
          />
        </div>
        <div className="flex flex-col gap-1 p-4 text-start">
          <h3 className="font-medium text-text-h">{product.name}</h3>
          <p className="text-sm text-text">
            {formatCurrency(product.dailyRate)} / день
          </p>
          <p className="text-xs text-text">
            Депозит: {formatCurrency(product.depositAmount)}
          </p>
        </div>
      </Card>
    </Link>
  );
}