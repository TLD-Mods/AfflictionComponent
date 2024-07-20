namespace AfflictionComponent.Patches;

internal static class StatusBarPatches
{
    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsDebuffActive))]
    private static class IsCustomAfflictionDebuffActive
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition) return;
            // Add logic here to determine whether the custom affliction is a debuff and if it's active.
        }
    }
    
    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsBuffActive))]
    private static class IsCustomAfflictionBuffActive
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition) return;
            // Add logic here to determine whether the custom affliction is a buff and if it's active.
        }
    }
}