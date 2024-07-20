namespace AfflictionComponent.Patches;

internal static class FirstAidPanelPatches
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            // Add logic here to determine whether a custom affliction is current at risk.
        }
    }
    
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasBadAffliction))]
    private static class HasCustomBadAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            // Add logic here to determine whether the player currently has a bad custom affliction.
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class AddCustomAfflictionsToScrollList
    {
        private static void Postfix() { }
    }
}