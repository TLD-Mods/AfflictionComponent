using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class HasRiskAffliction
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            if (__result) return;
            
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            if (!customAfflictions.Any(affliction => !affliction.HasBuff() && !affliction.HasRisk()))
            {
                __result = customAfflictions.Any(affliction => affliction.HasRisk());
            }
        }
    }
}