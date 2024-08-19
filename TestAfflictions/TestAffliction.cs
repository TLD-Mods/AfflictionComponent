using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.TestAfflictions;

internal class TestAffliction : CustomAffliction, IBuff, IRiskPercentage, IDuration, IRemedies
{
    private float m_RiskValue;
    private float m_LastUpdateTime;
 
    public bool Buff { get; set; }
    public float Duration { get; set; }
    public float EndTime { get; set; }
    public bool Risk { get; set; }
    
    public TestAffliction(string name, string causeText, string description, string? descriptionNoHeal, string? spriteName, AfflictionBodyArea location) : base(name, causeText, description, descriptionNoHeal, spriteName, location)
    {
        m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
    }

    public Tuple<string, int, int>[] AltRemedyItems { get; set; }
    public bool InstantHeal { get; set; }
    public Tuple<string, int, int>[] RemedyItems { get; set; }
    
    public void CureSymptoms()
    {
        throw new NotImplementedException();
    }

    public void OnCure()
    {
        throw new NotImplementedException();
    }

    public float GetRiskValue() => m_RiskValue;

    public override void OnUpdate()
    {
        if (Risk)
        {
            UpdateRiskValue();

            if (GetRiskValue() >= 100) Cure(false);
            else if (GetRiskValue() < 0f) Cure();
        }
            
        if (Buff)
            InterfaceDuration.UpdateBuffDuration();
    }

    public void UpdateRiskValue()
    {
        var currentTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
        var elapsedTime = currentTime - m_LastUpdateTime;
            
        var riskIncrease = elapsedTime * 60f;
            
        m_RiskValue = Mathf.Min(m_RiskValue + riskIncrease, 100f);
        m_LastUpdateTime = currentTime;
            
        // The UI seems to be updating about 0.5% quicker than what's being logged.
        // Mod.Logger.Log($"Risk for {m_AfflictionKey} increased to {m_RiskPercentage:F2}%", ComplexLogger.FlaggedLoggingLevel.Debug);
    }
}