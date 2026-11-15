import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type SpinnerSize = "sm" | "md" | "lg";

const sizes: Record<SpinnerSize, string> = {
  sm: "size-4 border-2",
  md: "size-6 border-2",
  lg: "size-8 border-[3px]",
};

export type SpinnerProps = ComponentProps<"span"> & {
  size?: SpinnerSize;
  label?: string;
};

export function Spinner({
  size = "md",
  label,
  className,
  ...props
}: SpinnerProps) {
  return (
    <span
      role={label ? "status" : undefined}
      aria-label={label}
      aria-hidden={label ? undefined : true}
      className={cn(
        "inline-block animate-spin rounded-full border-current border-t-transparent motion-reduce:animate-none",
        sizes[size],
        className,
      )}
      {...props}
    />
  );
}