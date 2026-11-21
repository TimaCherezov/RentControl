import { useState } from "react";
import { cn } from "../../utils/cn";

type ImageWithFallbackProps = {
  src?: string | null;
  alt?: string | null;
  name: string;
  className?: string;
};

export function ImageWithFallback({
  src,
  alt,
  name,
  className,
}: ImageWithFallbackProps) {
  const [failed, setFailed] = useState(false);

  if (!src || failed) {
    return (
      <div
        className={cn(
          "flex size-full items-center justify-center p-1 text-center text-xs text-text",
          className,
        )}
      >
        {alt || "Нет фото"}
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt ?? name}
      loading="lazy"
      onError={() => setFailed(true)}
      className={cn("size-full object-cover", className)}
    />
  );
}