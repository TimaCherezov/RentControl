using Main.Model.Models;

namespace Main.Api.Features.ProductUnits.Create;

public record CreateProductUnitCommand(
    Guid ProductId,
    StateEnum State,
    string SerialNumber,
    DateOnly PurchaseDate,
    decimal PurchasePrice,
    Guid LocationId
);
