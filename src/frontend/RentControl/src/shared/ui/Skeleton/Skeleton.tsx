import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

export type SkeletonProps = ComponentProps<"div">;

export function Skeleton({ className, ...props }: SkeletonProps) {
  return (
    <div
      aria-hidden
      className={cn(
        "animate-pulse rounded bg-border/60 motion-reduce:animate-none",
        className,
      )}
      {...props}
    />
  );
}