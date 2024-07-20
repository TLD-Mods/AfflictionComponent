using AfflictionComponent.Afflictions;

namespace AfflictionComponent.Patches;

internal static class PanelFirstAidPatches
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            foreach (var customAffliction in customAfflictions)
            {
                __result = customAffliction.HasAfflictionRisk();
            }
        }
    }
    
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasBadAffliction))]
    private static class HasCustomBadAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            // We need to add logic to determine if the current affliction is 'bad'.
            // Hinterland are doing this by checking affliction that AREN'T 'Risk' or 'Beneficial'.
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class AddCustomAfflictionsToScrollList
    {
        private static void Postfix() { }
    }
}