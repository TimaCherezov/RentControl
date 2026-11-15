import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type SelectSize = "sm" | "md";

const sizes: Record<SelectSize, string> = {
  sm: "h-8 px-2 text-sm",
  md: "h-10 px-3 text-sm",
};

export type SelectProps = Omit<ComponentProps<"select">, "size"> & {
  size?: SelectSize;
  invalid?: boolean;
};

export function Select({
  size = "md",
  invalid = false,
  className,
  children,
  ...props
}: SelectProps) {
  return (
    <select
      aria-invalid={invalid || undefined}
      className={cn(
        "w-full rounded-md border border-border bg-bg text-text focus-visible:border-accent focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50",
        invalid && "border-danger focus-visible:border-danger",
        sizes[size],
        className,
      )}
      {...props}
    >
      {children}
    </select>
  );
}