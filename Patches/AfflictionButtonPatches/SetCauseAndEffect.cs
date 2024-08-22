using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class SetCauseAndEffect
{
    // Testing for displaying custom icons
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.SetCauseAndEffect))]
    private static class Test1
    {
        private static void Postfix(AfflictionButton __instance, string causeStr, AfflictionType affType, AfflictionBodyArea location, int index, string effectName, string spriteName)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;
            
            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex());
            if (!customAffliction.m_CustomSprite) return;
            
            __instance.m_SpriteEffect.atlas = Mod.customAtlas;
        }
    }
}