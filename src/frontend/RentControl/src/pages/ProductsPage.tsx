import { Container, Pagination, Stack } from "../shared/ui";
import { useProducts } from "../features/products/models";
import { ProductList } from "../features/products/ui/ProductList";
import { useSearchParams } from "react-router-dom";

const PAGE_SIZE = 12;

export function ProductsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = Math.max(1, Number(searchParams.get("page") ?? 1));
  const { data, isLoading, error } = useProducts({
    pageNumber: page,
    pageSize: PAGE_SIZE,
  });

  const setPage = (nextPage: number) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev);
      if (nextPage <= 1) next.delete("page");
      else next.set("page", String(nextPage));
      return next;
    });
  };

  return (
    <Container className="py-8">
      <Stack gap="lg">
        <h1>Продукты</h1>
        <ProductList
          products={data?.items ?? []}
          isLoading={isLoading}
          error={error}
        />
        {data && data.totalPages > 1 && (
          <Pagination
            page={data.pageNumber}
            pageSize={data.pageSize}
            totalCount={data.totalCount}
            onPageChange={setPage}
          />
        )}
      </Stack>
    </Container>
  );
}