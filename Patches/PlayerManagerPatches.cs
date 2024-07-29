using AfflictionComponent.Components;

namespace AfflictionComponent.Patches;

internal static class PlayerManagerPatches
{
    // Copy and pasted this method from 2.02 mono code, this allows the information in the afflictions panel to change for some reason?
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.UseFirstAidItem))]
    internal static class UseFirstAidItemPatch
    {
        private static bool Prefix(PlayerManager __instance, GearItem gi, ref bool __result)
        {
            if (gi.m_StackableItem && GameManager.GetInventoryComponent().NumGearInInventory(gi.name) < gi.m_FirstAidItem.m_UnitsPerUse)
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_PillsRequiredValue").Replace("{num-pills}", gi.m_FirstAidItem.m_UnitsPerUse.ToString()), false, false);
                return false;
            }
            Panel_Affliction.AfflictionExclusions afflictionExclusions = Panel_Affliction.AfflictionExclusions.MajorWristSprain | Panel_Affliction.AfflictionExclusions.MustBeTreatable;
            Il2CppSystem.Collections.Generic.List<Affliction> list = new();
            Panel_Affliction.GetAllBadAfflictions(afflictionExclusions, list);
            if (list.Count == 0 && AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() == 0)
            {
                __instance.TreatAfflictionWithFirstAid(gi.m_FirstAidItem, Affliction.InvalidAffliction);
                return false;
            }
            InterfaceManager.GetPanel<Panel_Affliction>().Enable(true, list, gi.m_FirstAidItem);
            return false;
        }
    }
}