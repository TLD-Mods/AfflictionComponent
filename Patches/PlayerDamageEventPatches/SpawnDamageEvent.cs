using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PlayerDamageEventPatches;

internal static class SpawnDamageEvent
{
    // TODO: This is my logic of thinking, but seems to be hitting a null reference somewhere.
    /*[HarmonyPatch(typeof(PlayerDamageEvent), nameof(PlayerDamageEvent.SpawnDamageEvent))]
    private static class Test1
    {
        private static void Postfix(PlayerDamageEvent __instance, string damageEventName, string damageEventType, string iconName, Color tint, bool fadeout, float displayTime, float fadeoutTime)
        {
            for (var i = 0; i < InterfaceManager.GetPanel<Panel_HUD>().m_PlayerDamageEventsGrid.transform.childCount; i++)
            {
                var gameObject = InterfaceManager.GetPanel<Panel_HUD>().m_PlayerDamageEventsGrid.transform.GetChild(i).gameObject;
                if (gameObject.GetComponent<PlayerDamageEvent>())
                {
                    var playerDamageEvent = gameObject.GetComponent<PlayerDamageEvent>();
                    if (AfflictionManager.GetAfflictionManagerInstance().m_Afflictions.Any(customAffliction => iconName == customAffliction.m_Name && customAffliction.m_CustomSprite))
                    {
                        playerDamageEvent.m_Icon.atlas = Mod.customAtlas;
                        return;
                    }
                    
                    playerDamageEvent.m_Icon.atlas = __instance.m_Background.atlas;
                }
            }
        }
    }*/
}