import type { ReactNode } from "react";
import { cn } from "../../utils/cn";

export type EmptyStateProps = {
  title: string;
  description?: string;
  action?: ReactNode;
  className?: string;
};

export function EmptyState({
  title,
  description,
  action,
  className,
}: EmptyStateProps) {
  return (
    <div
      className={cn(
        "flex flex-col items-center gap-2 rounded-lg border border-dashed border-border p-8 text-center",
        className,
      )}
    >
      <p className="font-medium text-text-h">{title}</p>
      {description && <p className="text-sm text-text">{description}</p>}
      {action}
    </div>
  );
}