using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Patches.StatusBarPatches;

internal static class IsBuffActive
{
    [HarmonyPatch(nameof(StatusBar), nameof(StatusBar.IsBuffActive))]
    private static class IsCustomAfflictionBuffActive
    {
        private static void Postfix(StatusBar __instance, ref bool __result)
        {
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            var hasBuff = false;
            IBuff? interfaceBuff = null;
            
            foreach (var customAffliction in customAfflictions)
            {
                hasBuff = customAffliction.HasBuff();
                interfaceBuff = AfflictionManager.TryGetInterface<IBuff>(customAffliction);
            }
            
            if (__instance.m_StatusBarType == StatusBar.StatusBarType.Condition)
            {
                __result = hasBuff;
            }
            
            if (__instance.m_StatusBarType == StatusBar.StatusBarType.Fatigue)
            {
                if (hasBuff && interfaceBuff is not null && interfaceBuff.BuffFatigue) __result = true;
            }
            
            if (__instance.m_StatusBarType == StatusBar.StatusBarType.Cold)
            {
                if (hasBuff && interfaceBuff is not null && interfaceBuff.BuffCold) __result = true;
            }
            
            if (__instance.m_StatusBarType == StatusBar.StatusBarType.Hunger)
            {
                if (hasBuff && interfaceBuff is not null && interfaceBuff.BuffHunger) __result = true;
            }
            
            if (__instance.m_StatusBarType == StatusBar.StatusBarType.Thirst)
            {
                if (hasBuff && interfaceBuff is not null && interfaceBuff.BuffThirst) __result = true;
            }
        }
    }
}