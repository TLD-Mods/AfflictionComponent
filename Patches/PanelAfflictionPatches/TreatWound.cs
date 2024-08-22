using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelAfflictionPatches;

internal static class TreatWound
{
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.TreatWound))]
    private static class TreatWoundOverride
    {
        private static bool Prefix() => false;

        private static void Postfix(Panel_Affliction __instance)
        {
            GlobalFields.selectedCustomAffliction = null;

            Affliction afflictionSelected;
            if (!TryGetVanillaAffliction(out afflictionSelected, __instance))
            {
                int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
                int totalCount = GlobalFields.panelAfflictionList.Count + __instance.m_Afflictions.Count;

                if (tweenTargetIndex >= 0 && tweenTargetIndex < totalCount)
                {
                    GlobalFields.selectedCustomAffliction = GlobalFields.panelAfflictionList[tweenTargetIndex - __instance.m_Afflictions.Count];
                    if (GlobalFields.selectedCustomAffliction == null) return;
                }
                else return;
            }

            GameManager.GetPlayerManagerComponent().TreatAfflictionWithFirstAid(__instance.m_FirstAidItem, afflictionSelected);
            __instance.Enable(false, null, null);
        }

        private static bool TryGetVanillaAffliction(out Affliction affliction, Panel_Affliction __instance)
        {
            int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
            if (tweenTargetIndex >= 0 && tweenTargetIndex < __instance.m_Afflictions.Count)
            {
                affliction = __instance.m_Afflictions[tweenTargetIndex];
                return true;
            }
            
            affliction = Affliction.InvalidAffliction;
            return false;
        }
    }
}