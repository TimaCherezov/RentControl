import { Button } from "../Button";
import { cn } from "../../utils/cn";

export type PaginationProps = {
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  className?: string;
};

export function Pagination({
  page,
  pageSize,
  totalCount,
  onPageChange,
  className,
}: PaginationProps) {
  if (totalCount <= 0 || pageSize <= 0) {
    return null;
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <nav
      aria-label="Пагинация"
      className={cn("flex items-center justify-center gap-2", className)}
    >
      <Button
        variant="secondary"
        size="sm"
        disabled={page <= 1}
        onClick={() => onPageChange(page - 1)}
      >
        Назад
      </Button>
      <span className="text-sm text-text">
        Стр. {page} из {totalPages}
      </span>
      <Button
        variant="secondary"
        size="sm"
        disabled={page >= totalPages}
        onClick={() => onPageChange(page + 1)}
      >
        Вперёд
      </Button>
    </nav>
  );
}