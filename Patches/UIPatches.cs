using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace AfflictionComponent.Patches
{
    internal class UIPatches
    {

        [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]

        public class AddCustomAfflictionsToScrollList
        {

            public static void Postfix()
            {

            }

        }

        [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.SetupScrollList))]

        public class Panel_Afflictions_AddCustomAfflictionsToScrollList
        {

            public static bool Prefix()
            {
                return false;
            }
            public static void Postfix(ref Il2CppSystem.Collections.Generic.List<Affliction> afflictionList, Panel_Affliction __instance)
            {

                int combinedCount = __instance.m_Afflictions.Count + Main.afflictionManager.GetCustomAfflictionCount();

                if (afflictionList == null)
                {
                    return;
                }
                __instance.m_Afflictions = afflictionList;
                __instance.m_CoverflowAfflictions.Clear();
                __instance.m_ScrollList.CleanUp();
                __instance.m_ScrollList.CreateList(combinedCount);
                for (int i = 0; i < __instance.m_Afflictions.Count; i++)
                {
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[i]);
                    if (!(componentInChildren == null))
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);
                        if (__instance.m_Afflictions[i].IsValid())
                        {
                            componentInChildren.SetAffliction(__instance.m_Afflictions[i]);
                        }
                        else
                        {
                            componentInChildren.SetEmptySlot();
                        }
                    }
                }

                for (int j = __instance.m_Afflictions.Count; j < combinedCount; j++)
                {
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                    if (!(componentInChildren == null))
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);

                        componentInChildren.m_SpriteEffect.spriteName = Main.afflictionManager.m_Afflictions[j].GetSpriteName();

                    }
                }

            }

        }

    }
}
