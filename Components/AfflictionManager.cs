namespace AfflictionComponent.Components;

[RegisterTypeInIl2Cpp(false)]
internal class AfflictionManager : MonoBehaviour
{
    public List<CustomAffliction> m_Afflictions = [];

    public static AfflictionManager GetAfflictionManagerInstance() => Mod.afflictionManager;
    
    public int GetCustomAfflictionCount() => m_Afflictions.Count();

    // We should probably change this to an enum or something?
    
    /// <summary>
    /// Returns the colour based on the affliction.
    /// </summary>
    /// <param name="afflictionType">Choose from the following strings - "Buff", "Risk", "Bad"</param>
    /// <returns></returns>
    public static Color AfflictionColour(string afflictionType)
    {
        return afflictionType switch
        {
            "Buff" => InterfaceManager.m_FirstAidBuffColor,
            "Risk" => InterfaceManager.m_FirstAidRiskColor,
            "Bad" => InterfaceManager.m_FirstAidRedColor
        };
    }
    
    //so mod authors can check if the player has at least one CustomAffliction of their own type
    public bool HasAfflictionOfType(Type typeName) => m_Afflictions.Any(obj => typeName.IsAssignableFrom(obj.GetType()));

    public void Start()
    {
        var customAfflictionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(CustomAffliction)) && !type.IsAbstract);

        foreach (var afflictionType in customAfflictionTypes)
        {
            try
            {
                var constructor = afflictionType.GetConstructor([
                    typeof(string), typeof(AfflictionBodyArea), typeof(string), 
                    typeof(string), typeof(bool), typeof(bool), typeof(float), 
                    typeof(bool), typeof(bool), typeof(GearItem[])
                ]);

                if (constructor != null)
                {
                    var affliction = (CustomAffliction)constructor.Invoke([
                        "Default Cause", 
                        AfflictionBodyArea.Chest, 
                        "default_sprite",
                        "Default name",
                        false, // risk
                        false, // buff
                        24f,   // duration
                        false, // permanent
                        false, // instantHeal
                        Array.Empty<GearItem>() // remedyItems
                    ]);

                    m_Afflictions.Add(affliction);
                    MelonLogger.Msg($"Added affliction of type {afflictionType.Name}");
                }
                else
                {
                    MelonLogger.Warning($"Could not find appropriate constructor for affliction type {afflictionType.Name}");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error creating affliction of type {afflictionType.Name}: {ex.Message}");
            }
        }

        MelonLogger.Msg($"Initialized {m_Afflictions.Count} custom afflictions");
    }
    
    public void Update()
    {
        if (GameManager.m_IsPaused || GameManager.s_IsGameplaySuspended)
        {
            return;
        }

        //where the magic happens

        float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();

        for (int num = m_Afflictions.Count - 1; num >= 0; num--)
        {
            CustomAffliction affliction = m_Afflictions[num];

            affliction.OnUpdate();

            if (hoursPlayedNotPaused > affliction.m_EndTime)
            {
                affliction.Cure();
                InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
            }
        }
    }

    public void Add(CustomAffliction ca)
    {
        m_Afflictions.Add(ca);
    }

    public void Remove(CustomAffliction ca)
    {
        m_Afflictions.Remove(ca);
    }
}