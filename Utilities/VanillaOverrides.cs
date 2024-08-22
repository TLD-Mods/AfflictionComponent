using Il2CppTLD.IntBackedUnit;

namespace AfflictionComponent.Utilities;

public class VanillaOverrides
{
    public static void FoodPoisoningMethod(Panel_FirstAid __instance, int selectedAfflictionIndex, out int num, out int num4)
    {
        FoodPoisoning foodPoisoningComponent = GameManager.GetFoodPoisoningComponent();
        __instance.m_LabelAfflictionDescriptionNoRest.text = "";
        __instance.m_LabelAfflictionDescription.text = foodPoisoningComponent.m_Description;

        string[] remedySprites = ["GEAR_BottleAntibiotics"];
        bool[] remedyComplete = [foodPoisoningComponent.HasTakenAntibiotics()];
        int[] remedyNumRequired = [2];
        string[] altRemedySprites = ["GEAR_ReishiTea"];
        bool[] altRemedyComplete = [foodPoisoningComponent.HasTakenAntibiotics()];
        int[] altRemedyNumRequired = [1];

        __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, ItemLiquidVolume.Zero, foodPoisoningComponent.GetRestAmountRemaining(), foodPoisoningComponent.m_NumHoursRestForCure);
        num = (int)Panel_Affliction.GetAfflictionLocation(AfflictionType.FoodPoisioning, selectedAfflictionIndex);

        num4 = 0; // For compliance
    }

    public static void DysenteryMethod(Panel_FirstAid __instance, int selectedAfflictionIndex, out int num, out int num4)
    {

        Dysentery dysenteryComponent = GameManager.GetDysenteryComponent();
        __instance.m_LabelAfflictionDescriptionNoRest.text = "";
        __instance.m_LabelAfflictionDescription.text = dysenteryComponent.m_Description;

        string[] remedySprites = ["GEAR_WaterSupplyPotable", "GEAR_BottleAntibiotics"];
        bool[] remedyComplete = [dysenteryComponent.GetWaterAmountRemaining().m_Units < 10000000, dysenteryComponent.HasTakenAntibiotics()];
        int[] remedyNumRequired = [1, 2];
        string[] altRemedySprites = ["GEAR_WaterSupplyPotable", "GEAR_ReishiTea"];
        bool[] altRemedyComplete = [dysenteryComponent.GetWaterAmountRemaining().m_Units < 10000000, dysenteryComponent.HasTakenAntibiotics()];
        int[] altRemedyNumRequired = [1, 1];

        __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, dysenteryComponent.GetWaterAmountRemaining(), dysenteryComponent.GetRestAmountRemaining(), dysenteryComponent.m_NumHoursRestForCure);
        num = (int)Panel_Affliction.GetAfflictionLocation(AfflictionType.Dysentery, selectedAfflictionIndex);

        // For compliance
        num4 = 0;
    }
}