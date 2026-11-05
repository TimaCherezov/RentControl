using Main.Model.Models;

namespace Main.Api.Features.ProductUnits;

public record ProductUnitDto(
    Guid Id,
    Guid ProductId,
    StateEnum State,
    string SerialNumber,
    DateOnly PurchaseDate,
    decimal PurchasePrice,
    Guid LocationId);
