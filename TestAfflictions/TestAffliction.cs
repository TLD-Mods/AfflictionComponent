using AfflictionComponent.Components;

namespace AfflictionComponent.Dev;

internal class TestAffliction : CustomAffliction
{
    public TestAffliction(string cause, AfflictionBodyArea location, string spriteName, string afflictionName, bool risk, bool buff, float duration, bool permanent, bool instantHeal, Tuple<string, int>[] remedyItems) : base(cause, location, spriteName, afflictionName, risk, buff, duration, permanent, instantHeal, remedyItems)
    {
    }
    
    public override void OnUpdate()
    {
        //Mod.Logger.Log("Child class update is calling!", ComplexLogger.FlaggedLoggingLevel.Debug);
    }
}