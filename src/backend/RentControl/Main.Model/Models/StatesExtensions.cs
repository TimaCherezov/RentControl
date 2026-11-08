namespace Main.Model.Models;

public static class StatesExtensions
{
    public static IReadOnlySet<StateEnum> RentableStates = new HashSet<StateEnum>
    {
        StateEnum.Good,
        StateEnum.Excellent,
        StateEnum.Used,
        StateEnum.New,
    };

    public static bool IsRentable(StateEnum state) => RentableStates.Contains(state);
}
