using Il2CppInterop.Runtime.Attributes;

namespace AfflictionComponent.Components;

[RegisterTypeInIl2Cpp(false)]
internal class AfflictionManager : MonoBehaviour
{
    public List<CustomAffliction> m_Afflictions = [];

    [HideFromIl2Cpp]
    public void Add(CustomAffliction ca) => m_Afflictions.Add(ca);
    
    [HideFromIl2Cpp]
    public List<CustomAffliction> GetAfflictionsByBodyArea(AfflictionBodyArea bodyArea) => m_Afflictions.Where(a => a.m_Location == bodyArea).ToList();
    
    public static AfflictionManager GetAfflictionManagerInstance() => Mod.afflictionManager;
    
    /// <summary>
    /// Returns the colour based on the affliction.
    /// </summary>
    /// <param name="afflictionType">Choose from the following strings - "Buff", "Risk", "Bad"</param>
    /// <returns></returns>
    public static Color GetAfflictionColour(string afflictionType)
    {
        return afflictionType switch
        {
            "Buff" => InterfaceManager.m_FirstAidBuffColor,
            "Risk" => InterfaceManager.m_FirstAidRiskColor,
            "Bad" => InterfaceManager.m_FirstAidRedColor
        };
    }
    
    public int GetCustomAfflictionCount() => m_Afflictions.Count();
    
    public CustomAffliction GetAfflictionByIndex(int index) => m_Afflictions[index];

    //so mod authors can check if the player has at least one CustomAffliction of their own type
    [HideFromIl2Cpp]
    public bool HasAfflictionOfType(Type typeName) => m_Afflictions.Any(obj => typeName.IsAssignableFrom(obj.GetType()));

    [HideFromIl2Cpp]
    public void Remove(CustomAffliction ca) => m_Afflictions.Remove(ca);
    
    public void Start() { }
    
    public void Update()
    {
        if (GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended) return;
        
        //where the magic happens

        float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();

        for (int num = m_Afflictions.Count - 1; num >= 0; num--)
        {
            CustomAffliction affliction = m_Afflictions[num];

            if (GameManager.GetPlayerManagerComponent().m_God) affliction.Cure();
            
            affliction.OnUpdate();

            if (hoursPlayedNotPaused > affliction.m_EndTime)
            {
                affliction.Cure();
            }
        }
    }
}