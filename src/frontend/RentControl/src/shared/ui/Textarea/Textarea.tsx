import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type TextareaSize = "sm" | "md";

const sizes: Record<TextareaSize, string> = {
  sm: "min-h-16 px-2 py-1.5 text-sm",
  md: "min-h-24 px-3 py-2 text-sm",
};

export type TextareaProps = Omit<ComponentProps<"textarea">, "size"> & {
  size?: TextareaSize;
  invalid?: boolean;
};

export function Textarea({
  size = "md",
  invalid = false,
  className,
  ...props
}: TextareaProps) {
  return (
    <textarea
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