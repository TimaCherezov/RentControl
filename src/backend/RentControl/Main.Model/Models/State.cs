namespace Main.Model.Models;

/// <summary>
/// Состояние продукта.
/// </summary>
public class State
{
    public StateEnum Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public enum StateEnum
{
    NEW = 1,
    EXCELLENT = 2,
    GOOD = 3,
    USED = 4,
    NEEDS_MAINTENANCE = 5,
    NEEDS_REPAIR = 6,
    BROKEN = 7,
    WRITE_OFF = 8,
}
