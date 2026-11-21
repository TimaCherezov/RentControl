import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type InputSize = "sm" | "md";

const sizes: Record<InputSize, string> = {
  sm: "h-8 px-2 text-sm",
  md: "h-10 px-3 text-sm",
};

export type InputProps = Omit<ComponentProps<"input">, "size"> & {
  size?: InputSize;
  invalid?: boolean;
};

export function Input({
  size = "md",
  invalid = false,
  className,
  ...props
}: InputProps) {
  return (
    <input
      aria-invalid={invalid || undefined}
      className={cn(
        "w-full rounded-md border border-border bg-bg text-text placeholder:text-text/60 focus-visible:border-accent focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50",
        invalid && "border-danger focus-visible:border-danger",
        sizes[size],
        className,
      )}
      {...props}
    />
  );
}