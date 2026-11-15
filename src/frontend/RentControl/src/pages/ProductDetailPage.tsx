import { useNavigate, useParams } from "react-router-dom";
import {
  Button,
  Container,
  EmptyState,
  Skeleton,
  Stack,
} from "../shared/ui";
import { useProduct } from "../features/products/models";
import { ProductGallery } from "../features/products/ui/ProductGallery";
import { ProductDetails } from "../features/products/ui/ProductDetails";
import { ProductDescription } from "../features/products/ui/ProductDescription";

export function ProductDetailPage() {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const { data: product, isLoading, error } = useProduct(id);

  if (isLoading) {
    return (
      <Container className="py-8">
        <Skeleton className="h-96 rounded-lg" />
      </Container>
    );
  }

  if (error || !product) {
    return (
      <Container className="py-8">
        <EmptyState
          title="Продукт не найден"
          description={
            error instanceof Error ? error.message : "Возможно, он удалён."
          }
          action={
            <Button variant="secondary" onClick={() => navigate("/products")}>
              К списку
            </Button>
          }
        />
      </Container>
    );
  }

  return (
    <Container className="py-8">
      <Stack gap="lg">
        <div className="flex items-center">
          <Button
            variant="primary"
            aria-label="Назад к каталогу"
            className="size-10 px-0"
            onClick={() => navigate("/products")}
          >
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden
            >
              <path d="M19 12H5" />
              <path d="m12 19-7-7 7-7" />
            </svg>
          </Button>
        </div>

        <div className="grid gap-6 lg:grid-cols-2 lg:items-start">
          <ProductGallery images={product.images} name={product.name} />
          <Stack gap="md">
            <ProductDetails product={product} />
            <Button
              fullWidth
              onClick={() =>
                navigate(`/bookings/new?productId=${product.id}`)
              }
            >
              Забронировать
            </Button>
          </Stack>
        </div>

        <ProductDescription product={product} />
      </Stack>
    </Container>
  );
}