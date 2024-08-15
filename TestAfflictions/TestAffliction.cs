using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.TestAfflictions;

internal class TestAffliction : CustomAffliction, IBuff, IRiskPercentage, IDuration
{
    private float m_RiskValue;
    private float m_LastUpdateTime;
 
    public bool Buff { get; set; }
    
    public float Duration { get; set; }
    
    public float EndTime { get; set; }
    
    public bool Risk { get; set; }
    
    public TestAffliction(string afflictionName, string cause, string description, string? descriptionNoHeal, string spriteName, AfflictionBodyArea location, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : base(afflictionName, cause, description, descriptionNoHeal, spriteName, location, instantHeal, remedyItems, altRemedyItems)
    {
        m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
    }

    protected override void CureSymptoms() { }
    
    public float GetRiskValue() => m_RiskValue;

    public override void OnUpdate()
    {
        if (Risk)
            UpdateRiskValue();
        if (Buff)
            InterfaceDuration.UpdateBuffDuration();
    }

    protected override void OnCure() { }

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