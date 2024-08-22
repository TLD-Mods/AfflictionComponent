namespace AfflictionComponent.Patches.PlayerDamageEventPatches;

internal static class SpawnDamageEvent
{
    // TODO: Need to somehow figure out how to capture a local variable within, if possible?
    [HarmonyPatch(typeof(PlayerDamageEvent), nameof(PlayerDamageEvent.SpawnDamageEvent))]
    private static class Test1
    {
        private static void Postfix(PlayerDamageEvent __instance, string damageEventName, string damageEventType, string iconName, Color tint, bool fadeout, float displayTime, float fadeoutTime) { }
    }
}