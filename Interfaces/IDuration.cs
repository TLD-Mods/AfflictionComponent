namespace AfflictionComponent.Interfaces;

public interface IDuration
{
    public float Duration { get; set; } // In Hours.
    
    public float EndTime { get; set; }

    public sealed float GetTimeRemaining() => (EndTime - GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused()) * 60f;

    /**
    public sealed void UpdateBuffDuration() => Duration = EndTime - GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
    **/

    public sealed bool IsDurationUp() => GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() > EndTime;
}