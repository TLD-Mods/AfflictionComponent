using AfflictionComponent.Components;
using AfflictionComponent.Utilities;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class SetCauseAndEffect
{
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.SetCauseAndEffect))]
    private static class SwapAfflictionButtonIconToCustomAtlas
    {
        private static void Postfix(AfflictionButton __instance, string causeStr, AfflictionType affType, AfflictionBodyArea location, int index, string effectName, string spriteName)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;
            
            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex());
            if (!customAffliction.m_CustomSprite) return;

            // This is here so if the affliction is already applied to the player, it needs to be loaded again as Start() wasn't called.
            AtlasUtilities.AddCustomSpriteToAtlas(customAffliction.m_SpriteName);
            __instance.m_SpriteEffect.atlas = Mod.customAtlas;
        }
    }
}