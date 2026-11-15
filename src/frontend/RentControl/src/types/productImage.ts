export type ProductImageDto = {
  id: string;
  productId: string;
  isMain: boolean;
  createdAt: string;
  altText: string | null;
  url: string;
};

export type CreateProductImageRequest = {
  productId: string;
  storageKey: string;
  altText?: string;
};

export type UpdateProductImageRequest = {
  storageKey: string;
  isMain: boolean;
  altText?: string;
};
