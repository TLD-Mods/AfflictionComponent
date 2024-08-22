using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class HasBadAffliction
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasBadAffliction))]
    private static class HasCustomBadAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            if (__result) return;
            
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            __result = customAfflictions.Any(affliction => !affliction.HasBuff() && !affliction.HasRisk());
        }
    }
}