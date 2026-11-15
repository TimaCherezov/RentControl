import type { ComponentProps } from "react";
import { cn } from "../../utils/cn";

export type ContainerProps = ComponentProps<"div">;

export function Container({ className, ...props }: ContainerProps) {
  return (
    <div
      className={cn("mx-auto w-full max-w-[1126px] px-4", className)}
      {...props}
    />
  );
}