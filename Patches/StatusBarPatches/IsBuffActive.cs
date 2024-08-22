using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.StatusBarPatches;

internal static class IsBuffActive
{
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
                __result = customAffliction.HasBuff();
            }
        }
    }
}