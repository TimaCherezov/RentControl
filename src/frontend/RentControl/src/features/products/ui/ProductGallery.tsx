import { useState } from "react";
import { cn } from "../../../shared/utils/cn";
import { ImageWithFallback } from "../../../shared/ui";
import type { ProductImageDto } from "../../../types/productImage";

type ProductGalleryProps = {
  images: ProductImageDto[];
  name: string;
};

export function ProductGallery({ images, name }: ProductGalleryProps) {
  const [selected, setSelected] = useState(0);

  if (images.length === 0) {
    return (
      <div className="flex aspect-[4/3] items-center justify-center rounded-lg border border-border bg-accent-bg text-text">
        Нет фото
      </div>
    );
  }

  const active = images[selected] ?? images[0];

  return (
    <div className="flex flex-col gap-2">
      <div className="aspect-[4/3] w-full overflow-hidden rounded-lg border border-border bg-accent-bg">
        <ImageWithFallback
          key={active.id}
          src={active.url}
          alt={active.altText}
          name={name}
        />
      </div>
      {images.length > 1 && (
        <div className="flex flex-wrap gap-2">
          {images.map((image, index) => (
            <button
              key={image.id}
              type="button"
              aria-label={`Фото ${index + 1}`}
              onClick={() => setSelected(index)}
              className={cn(
                "size-16 overflow-hidden rounded-md border",
                index === selected ? "border-accent" : "border-border",
              )}
            >
              <ImageWithFallback
                src={image.url}
                alt={image.altText}
                name={name}
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}