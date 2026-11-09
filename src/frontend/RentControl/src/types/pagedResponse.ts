export class PagedResponse<T> {
  readonly items: T[];
  readonly totalCount: bigint;
  readonly pageNumber: number;
  readonly pageSize: number;

  constructor(
    items: T[],
    totalCount: bigint,
    pageNumber: number,
    pageSize: number,
  ) {
    this.items = items;
    this.totalCount = totalCount;
    this.pageNumber = pageNumber;
    this.pageSize = pageSize;
  }

  get totalPages(): number {
    return Number(Math.ceil(Number(this.totalCount) / this.pageSize));
  }

  get hasPrevious(): boolean {
    return this.pageNumber > 1;
  }

  get hasNext(): boolean {
    return this.pageNumber < this.totalPages;
  }
}
