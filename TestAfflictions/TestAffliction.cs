using AfflictionComponent.Components;

namespace AfflictionComponent.Dev;

internal class TestAffliction : CustomAffliction
{
    public TestAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : base(afflictionName, cause, desc, noHealDesc, location, spriteName, risk, buff, duration, noTimer, instantHeal, remedyItems, altRemedyItems)
    {
    }

    public override void OnUpdate()
    {
        //Mod.Logger.Log("Child class update is calling!", ComplexLogger.FlaggedLoggingLevel.Debug);
    }

    public override void OnCure()
    {
    }

    public override void CureSymptoms()
    {
        Mod.Logger.Log("Curing symptoms only...", ComplexLogger.FlaggedLoggingLevel.Debug);
    }
}