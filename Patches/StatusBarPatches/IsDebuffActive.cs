using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.StatusBarPatches;

internal static class IsDebuffActive
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
                __result = customAffliction.HasRisk() || !customAffliction.HasBuff();
            }
        }
    }
}