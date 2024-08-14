using AfflictionComponent.Components;

namespace AfflictionComponent.Patches;

internal static class StatusBarPatches
{
    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsDebuffActive))]
    private static class IsCustomAfflictionDebuffActive
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition) return;
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            
            foreach (var customAffliction in customAfflictions)
            {
                __result = customAffliction.HasAfflictionRisk() || !customAffliction.m_Buff;
            }
        }
    }
    
    // TODO: Need to add in checks for specific status bars such as hunger, thirst if this buff affects those.
    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsBuffActive))]
    private static class IsCustomAfflictionBuffActive
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition) return;
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            
            foreach (var customAffliction in customAfflictions)
            {
                __result = customAffliction.m_Buff;
            }
        }
    }
}