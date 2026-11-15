import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

type StackDirection = "row" | "column";
type StackGap = "sm" | "md" | "lg";

const directions: Record<StackDirection, string> = {
  row: "flex-row",
  column: "flex-col",
};

const gaps: Record<StackGap, string> = {
  sm: "gap-2",
  md: "gap-4",
  lg: "gap-6",
};

export type StackProps = ComponentProps<"div"> & {
  direction?: StackDirection;
  gap?: StackGap;
};

export function Stack({
  direction = "column",
  gap = "md",
  className,
  ...props
}: StackProps) {
  return (
    <div
      className={cn("flex", directions[direction], gaps[gap], className)}
      {...props}
    />
  );
}