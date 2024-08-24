﻿using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PlayerDamageEventPatches;

internal static class SpawnDamageEvent
{
    // TODO: This basically works, however if a custom affliction is last to show up with a custom icon - then it's blank for some reason.
    [HarmonyPatch(typeof(PlayerDamageEvent), nameof(PlayerDamageEvent.SpawnDamageEvent))]
    private static class PlayerDamageEventPatch
    {
        private static void Postfix(PlayerDamageEvent __instance, string iconName)
        {
            var hudPanel = InterfaceManager.GetPanel<Panel_HUD>();
            var damageEvent = hudPanel.m_PlayerDamageEventsGrid.transform.GetChild(hudPanel.m_PlayerDamageEventsGrid.transform.childCount - 1).GetComponent<PlayerDamageEvent>();

            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions.FirstOrDefault(a => a.m_SpriteName == iconName && a.m_CustomSprite);

            if (customAffliction != null)
            {
                // Not sure if this works or not, as the UIAtlas component isn't being attached at all.
                for (var i = 0; i < Mod.allCustomAtlas.transform.childCount; i++)
                {
                    if ($"CustomAtlas{customAffliction.m_SpriteName}(Clone)" == Mod.allCustomAtlas.transform.GetChild(i).name) __instance.m_Icon.atlas = Mod.allCustomAtlas.transform.GetChild(i).GetComponent<UIAtlas>();
                }

                damageEvent.m_Icon.spriteName = iconName;
            }
            else
            {
                damageEvent.m_Icon.atlas = damageEvent.m_Background.atlas;
                damageEvent.m_Icon.spriteName = iconName;
            }
        }
    }
}