namespace AfflictionComponent.Interfaces;

public interface IDuration
{
    internal float Duration { get; } // In Hours.
    
    internal float EndTime { get; set; }

    public sealed float GetTimeRemaining() => Mathf.CeilToInt(Duration * 60f);
}