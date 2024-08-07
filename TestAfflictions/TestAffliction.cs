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

    public float GetRiskValue() => m_RiskPercentage;

    public override void OnUpdate()
    {
        if (HasAfflictionRisk())
        {
            float currentTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            float elapsedTime = currentTime - m_LastUpdateTime;
            
            float riskIncrease = elapsedTime * 60f;
            
            m_RiskPercentage = Mathf.Min(m_RiskPercentage + riskIncrease, 100f);
            m_LastUpdateTime = currentTime;
            
            Mod.Logger.Log($"Risk for {m_AfflictionKey} increased to {m_RiskPercentage:F2}%", ComplexLogger.FlaggedLoggingLevel.Debug);
        }
    }

    public override void OnCure()
    {
    }

    public override void CureSymptoms()
    {
        Mod.Logger.Log("Curing symptoms only...", ComplexLogger.FlaggedLoggingLevel.Debug);
    }
}