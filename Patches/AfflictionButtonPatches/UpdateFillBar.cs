using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class UpdateFillBar
{
    // Index was out of range error occurs in this patch when the percentage reaches 100% and is removed from the m_Afflictions list in AfflictionManager.
    // The cause of the problem with the directional indicator seems to be the animator, I don't know why it's treating our custom affliction differently though.
    [HarmonyPatch(typeof(AfflictionButton), nameof(AfflictionButton.UpdateFillBar))]
    private static class UpdateFillBarCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;

            if (!AfflictionManager.GetAfflictionManagerInstance().TryGetAfflictionByIndex(__instance.GetAfflictionIndex(), out var customAffliction) || customAffliction == null) return;
            
            var riskPercentage = AfflictionManager.TryGetInterface<IRiskPercentage>(customAffliction);
            if (riskPercentage != null && riskPercentage.Risk)
            {
                var num = riskPercentage.GetRiskValue() / 100f; // The 100f slows it down, otherwise right now it's way too quick.
                Utils.SetActive(__instance.m_AnimatorAfflictionBar.gameObject, num > 0f);
                __instance.m_FillSpriteAfflictionBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num);
                __instance.m_SizeModifierAfflictionBar.localScale = new Vector3(num, 1f, 1f);
            }

            if (customAffliction.HasBuff())
            {
                var interfaceDuration = AfflictionManager.TryGetInterface<IDuration>(customAffliction);
                if (interfaceDuration != null)
                {
                    var num2 = interfaceDuration.GetTimeRemaining() / (interfaceDuration.Duration * 60f);
                    Utils.SetActive(__instance.m_AnimatorBuffBar.gameObject, num2 > 0f);
                    __instance.m_FillSpriteBuffBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num2);
                    __instance.m_SizeModifierBuffBar.localScale = new Vector3(num2, 1f, 1f);
                }
            }
        }
    }
}
