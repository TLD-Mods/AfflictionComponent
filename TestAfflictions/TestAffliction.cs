using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.TestAfflictions;

internal class TestAffliction : CustomAffliction, IRiskPercentage
{
    private float m_RiskPercentage;
    private float m_LastUpdateTime;
    
    public TestAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : base(afflictionName, cause, desc, noHealDesc, location, spriteName, risk, buff, duration, noTimer, instantHeal, remedyItems, altRemedyItems)
    {
        m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
    }

    protected override void CureSymptoms() { }
    
    public float GetRiskValue() => m_RiskPercentage;

    public override void OnUpdate()
    {
        if (HasAfflictionRisk())
            UpdateRiskValue();
    }

    protected override void OnCure() { }

    public void UpdateRiskValue()
    {
        var currentTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
        var elapsedTime = currentTime - m_LastUpdateTime;
            
        var riskIncrease = elapsedTime * 60f;
            
        m_RiskPercentage = Mathf.Min(m_RiskPercentage + riskIncrease, 100f);
        m_LastUpdateTime = currentTime;
            
        // Mod.Logger.Log($"Risk for {m_AfflictionKey} increased to {m_RiskPercentage:F2}%", ComplexLogger.FlaggedLoggingLevel.Debug); // The UI seems to be updating about 0.5% quicker than what's being logged.
    }
}