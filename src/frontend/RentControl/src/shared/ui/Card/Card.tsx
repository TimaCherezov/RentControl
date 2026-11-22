import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type CardPadding = "none" | "sm" | "md";

const paddings: Record<CardPadding, string> = {
  none: "",
  sm: "p-3",
  md: "p-4",
};

export type CardProps = ComponentProps<"div"> & {
  padding?: CardPadding;
};

export function Card({ padding = "md", className, ...props }: CardProps) {
  return (
    <div
      className={cn(
        "rounded-lg border border-border bg-bg shadow-card",
        paddings[padding],
        className,
      )}
      {...props}
    />
  );
}