export type LocationDto = {
  id: string;
  name: string;
  address: string;
};

export type CreateLocationRequest = {
  name: string;
  address: string;
};

export type UpdateLocationRequest = CreateLocationRequest;

export type GetPageLocationRequest = {
  pageNumber: number;
  pageSize: number;
};
