import { Card } from "../../../shared/ui";
import { formatCurrency } from "../../../shared/utils/format";
import type { ProductDto } from "../../../types/product";

export function ProductDetails({ product }: { product: ProductDto }) {
  const rates: Array<{ label: string; value: number }> = [
    { label: "В час", value: product.hourlyRate },
    { label: "В день", value: product.dailyRate },
  ];

  if (typeof product.weeklyRate === "number") {
    rates.push({ label: "В неделю", value: product.weeklyRate });
  }

  if (typeof product.monthlyRate === "number") {
    rates.push({ label: "В месяц", value: product.monthlyRate });
  }

  return (
    <Card className="flex flex-col gap-4">
      <h2>{product.name}</h2>
      <dl className="grid grid-cols-2 gap-3 sm:grid-cols-3">
        <div>
          <dt className="text-xs text-text">Рыночная стоимость</dt>
          <dd className="font-medium text-text-h">
            {formatCurrency(product.marketValue)}
          </dd>
        </div>
        <div>
          <dt className="text-xs text-text">Депозит</dt>
          <dd className="font-medium text-text-h">
            {formatCurrency(product.depositAmount)}
          </dd>
        </div>
        {rates.map((rate) => (
          <div key={rate.label}>
            <dt className="text-xs text-text">{rate.label}</dt>
            <dd className="font-medium text-text-h">
              {formatCurrency(rate.value)}
            </dd>
          </div>
        ))}
      </dl>
    </Card>
  );
}