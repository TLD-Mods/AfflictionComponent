using AfflictionComponent.Interfaces;
using Il2CppInterop.Runtime.Attributes;

namespace AfflictionComponent.Components;

[RegisterTypeInIl2Cpp(false)]
public class AfflictionManager : MonoBehaviour
{
    public List<CustomAffliction> m_Afflictions = [];

    [HideFromIl2Cpp]
    public void Add(CustomAffliction customAffliction) => m_Afflictions.Add(customAffliction);
    
    [HideFromIl2Cpp]
    public (bool hasMultiple, int count, int index) CheckMultipleAfflictionsByKey(string afflictionKey, CustomAffliction currentAffliction)
    {
        var afflictionsOfType = m_Afflictions.Where(affliction => affliction.m_Name == afflictionKey).ToList();
        var count = afflictionsOfType.Count;
        
        return (count > 1, count, afflictionsOfType.IndexOf(currentAffliction) + 1);
    }
    
    public static AfflictionManager GetAfflictionManagerInstance() => Mod.afflictionManager;

    public int GetCustomAfflictionCount() => m_Afflictions.Count;

    [HideFromIl2Cpp]
    public CustomAffliction GetAfflictionByIndex(int index) => m_Afflictions[index];

    [HideFromIl2Cpp]
    public List<CustomAffliction> GetAfflictionsByBodyArea(AfflictionBodyArea bodyArea) => m_Afflictions.Where(customAffliction => customAffliction.m_Location == bodyArea).ToList();

    /// <summary>
    /// Returns the colour based on the affliction.
    /// </summary>
    /// <param name="afflictionType">Choose from the following strings - "Buff", "Risk", "Bad"</param>
    /// <returns></returns>
    public static Color GetAfflictionColour(string afflictionType) => afflictionType switch
    {
        "Buff" => InterfaceManager.m_FirstAidBuffColor,
        "Risk" => InterfaceManager.m_FirstAidRiskColor,
        "Bad" => InterfaceManager.m_FirstAidRedColor,
        _ => throw new ArgumentException("Invalid affliction type", nameof(afflictionType))
    };

    [HideFromIl2Cpp]
    public List<CustomAffliction> GetCustomAfflictionListCurable() => m_Afflictions.Where(ca => ca.m_RemedyItems.Length > 0 && ca.NeedsRemedy()).ToList();
    
    [HideFromIl2Cpp] // So mod authors can check if the player has at least one CustomAffliction of their own type.
    public bool HasAfflictionOfType(Type typeName) => m_Afflictions.Any(typeName.IsInstanceOfType);

    [HideFromIl2Cpp]
    public void Remove(CustomAffliction customAffliction) => m_Afflictions.Remove(customAffliction);

    internal static T? TryGetInterface<T>(object obj) where T : class
    {
        if (obj is T interfaceInstance) return interfaceInstance;
        return null;
    }
    
    public void Update()
    {
        if (GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;

        for (int i = m_Afflictions.Count - 1; i >= 0; i--)
        {
            var customAffliction = m_Afflictions[i];

            if (GameManager.GetPlayerManagerComponent().m_God) 
                customAffliction.Cure();
            
            // This throws an out of index range error if you are in the FirstAid panel when it removes our custom affliction.
            // UPDATE: the Cure() method should solve this issue, HAVEN'T tested it yet!
            if (customAffliction is IRiskPercentage riskPercentage && riskPercentage.GetRiskValue() >= 100f)
                customAffliction.Cure();
            
            customAffliction.OnUpdate();

            if (customAffliction.HasDuration())
            { 
                if(customAffliction.InterfaceDuration.Duration < 0f) customAffliction.Cure();
            }
        }
    }
}