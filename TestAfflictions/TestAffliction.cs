using AfflictionComponent.Components;

namespace AfflictionComponent.Dev;

internal class TestAffliction : CustomAffliction
{
    public TestAffliction(string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, string afflictionName, bool risk, bool buff, float duration, bool permanent, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems) : base(cause, desc, noHealDesc, location, spriteName, afflictionName, risk, buff, duration, permanent, instantHeal, remedyItems, altRemedyItems)
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