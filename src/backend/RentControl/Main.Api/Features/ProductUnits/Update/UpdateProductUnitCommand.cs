using Main.Model.Models;

namespace Main.Api.Features.ProductUnits.Update;

public record UpdateProductUnitCommand(
    Guid Id,
    Guid ProductId,
    StateEnum State,
    string SerialNumber,
    DateOnly PurchaseDate,
    decimal PurchasePrice,
    Guid LocationId
);
