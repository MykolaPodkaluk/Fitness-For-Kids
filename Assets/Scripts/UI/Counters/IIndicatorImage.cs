public interface IIndicatorImage
{
    IndicatorState State { get; }
    void SetState(IndicatorState state);
}

public enum IndicatorState
{
    Active,
    Inactive,
    Completed
}