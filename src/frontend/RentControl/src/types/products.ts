import Big from "big.js";

export class ProductDto {
  readonly id: string;
  readonly name: string;
  readonly marketValue: Big;
  readonly depositOverride?: Big;
  readonly dailyRate: Big;
  readonly hourlyRate: Big;
  readonly weeklyRate?: Big;
  readonly monthlyRate?: Big;
  readonly description?: string;

  constructor(
    id: string,
    name: string,
    marketValue: Big,
    dailyRate: Big,
    hourlyRate: Big,
    depositOverride?: Big,
    weeklyRate?: Big,
    monthlyRate?: Big,
    description?: string,
  ) {
    this.id = id;
    this.name = name;
    this.marketValue = marketValue;
    this.dailyRate = dailyRate;
    this.hourlyRate = hourlyRate;
    this.depositOverride = depositOverride;
    this.weeklyRate = weeklyRate;
    this.monthlyRate = monthlyRate;
    this.description = description;
  }
}

export class CreateProductRequest {}

export class UpdateProductRequest {}

export class GetPageProductRequest {
  pageNumber: number;
  pageSize: number;
}
