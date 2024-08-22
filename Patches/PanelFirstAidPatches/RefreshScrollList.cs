using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class RefreshScrollList
{
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class RefreshScrollListOverride
    {
        private static bool Prefix(Panel_FirstAid __instance)
        {
            bool hasSelectedButton = __instance.m_SelectedAffButton != null;
            AfflictionType selectedType = hasSelectedButton ? __instance.m_SelectedAffButton.m_AfflictionType : AfflictionType.BloodLoss;
            AfflictionBodyArea selectedArea = hasSelectedButton ? __instance.m_SelectedAffButton.m_AfflictionLocation : AfflictionBodyArea.ArmLeft;

            __instance.m_ScrollListEffects.CleanUp();
            __instance.ClearAfflictionsAtLocationArray();
            Panel_Affliction.GetAllAfflictions(__instance.m_ScrollListAfflictions);

            foreach (var uiSprite in __instance.m_BodyIconList)
            {
                uiSprite.gameObject.SetActive(false);
            }

            List<CustomAffliction> customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            int totalAfflictions = __instance.m_ScrollListAfflictions.Count + customAfflictions.Count;

            __instance.m_ScrollListEffects.CreateList(totalAfflictions);
            AfflictionType lastAfflictionType = AfflictionType.BloodLoss;
            int count = 0;

            for (int i = 0; i < totalAfflictions; i++)
            {
                var afflictionButton = __instance.m_ScrollListEffects.m_ScrollObjects[i].transform.GetChild(0).GetComponent<AfflictionButton>();
                if (afflictionButton == null) continue;

                if (i < __instance.m_ScrollListAfflictions.Count)
                {
                    Affliction affliction = __instance.m_ScrollListAfflictions[i];
                    ProcessAffliction(__instance, affliction, afflictionButton, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
                else
                {
                    CustomAffliction customAffliction = customAfflictions[i - __instance.m_ScrollListAfflictions.Count];
                    ProcessCustomAffliction(__instance, customAffliction, afflictionButton, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
            }

            return false;
        }

        private static void ProcessAffliction(Panel_FirstAid instance, Affliction affliction, AfflictionButton component, ref AfflictionType lastAfflictionType, ref int count, bool hasSelectedButton, AfflictionType selectedType, AfflictionBodyArea selectedArea)
        {
            int location = (int)affliction.m_Location;
            instance.AddAfflictionAtLocation(location, affliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            if (affliction.m_AfflictionType != lastAfflictionType) count = 0;

            string text = Affliction.LocalizedNameFromAfflictionType(affliction.m_AfflictionType, count);
            string text2 = Affliction.SpriteNameFromAfflictionType(affliction.m_AfflictionType);
            component.SetCauseAndEffect(affliction.m_Cause, affliction.m_AfflictionType, affliction.m_Location, count, text, text2);
            count++;
            lastAfflictionType = affliction.m_AfflictionType;

            bool isSelected = hasSelectedButton && affliction.m_AfflictionType == selectedType && affliction.m_Location == selectedArea;
            component.SetSelected(isSelected);
            instance.UpdateBodyIconColors(component, isSelected, location);
        }

        private static void ProcessCustomAffliction(Panel_FirstAid instance, CustomAffliction customAffliction, AfflictionButton component, ref AfflictionType lastAfflictionType, ref int count, bool hasSelectedButton, AfflictionType selectedType, AfflictionBodyArea selectedArea)
        {
            int location = (int)customAffliction.m_Location;
            AddCustomAfflictionAtLocation(instance, location, customAffliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            AfflictionType afflictionType = AfflictionType.Generic;
            if (afflictionType != lastAfflictionType) count = 0;

            string text = customAffliction.m_Name;
            string text2 = customAffliction.m_SpriteName;
            component.SetCauseAndEffect(customAffliction.m_CauseText, afflictionType, customAffliction.m_Location, count, text, text2);
            count++;
            lastAfflictionType = afflictionType;

            bool isSelected = hasSelectedButton && afflictionType == selectedType && customAffliction.m_Location == selectedArea;
            component.SetSelected(isSelected);
            instance.UpdateBodyIconColors(component, isSelected, location);
        }
        
        private static void AddCustomAfflictionAtLocation(Panel_FirstAid instance, int bodyIconIndex, CustomAffliction customAffliction)
        {
            Panel_FirstAid.AfflictionsAtLocation afflictionsAtLocation = instance.m_AfflictionsAtLocationArray[bodyIconIndex];
            if (afflictionsAtLocation == null)
            {
                afflictionsAtLocation = new Panel_FirstAid.AfflictionsAtLocation(customAffliction.m_Location);
                instance.m_AfflictionsAtLocationArray[bodyIconIndex] = afflictionsAtLocation;
            }
            afflictionsAtLocation.AddAffliction(AfflictionType.Generic); // Adding this fixes index out of bounds error
        }
    }
}