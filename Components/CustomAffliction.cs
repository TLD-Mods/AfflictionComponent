namespace AfflictionComponent.Components;

// TODO: Split certain functionality off into Interfaces, which developers can then extend onto their custom afflictions.
public abstract class CustomAffliction
{
    public string m_AfflictionKey;
    public Tuple<string, int, int>[] m_AltRemedyItems;
    public bool m_BloodLoss; // The affliction causes blood loss, might not use, intended to override the vanilla Blood Loss affliction.
    public bool m_Buff;
    public string m_Cause;
    public string m_Desc;
    private float m_Duration; // In Hours.
    public float m_EndTime;
    private float m_StartEndTime;
    private bool m_InstantHeal;
    public AfflictionBodyArea m_Location;
    public string m_NoHealDesc;
    public bool m_NoTimer;
    public Tuple<string, int, int>[] m_RemedyItems; // GearItem, Required Amount, Current Amount.
    private bool m_Risk;
    private string m_SpriteName;

    protected CustomAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems)
    {
        m_Cause = cause; 
        m_Desc = desc;
        m_NoHealDesc = noHealDesc;
        m_Location = location;
        m_SpriteName = spriteName;
        m_AfflictionKey = afflictionName;
        m_Risk = risk;
        m_Buff = buff;
        m_Duration = duration;
        m_NoTimer = noTimer;
        m_InstantHeal = instantHeal;
        m_RemedyItems = remedyItems;
        m_AltRemedyItems = altRemedyItems;
        
        if (m_AltRemedyItems.Length > 0 && m_RemedyItems.Length == 0) // You can't have alternate remedy items if the main remedy items is blank.
        {
            m_RemedyItems = m_AltRemedyItems;
            m_AltRemedyItems = [];
        }

        if (m_Buff) // Buff takes precedence over risk if incorrectly assigned, they also cannot have remedy items.
        {
            m_Risk = false;
            m_RemedyItems = m_AltRemedyItems = [];
        }

        if (m_Risk) m_NoTimer = true;
        if (m_NoTimer) m_Duration = float.PositiveInfinity;
        
        Start();
    }

    private void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God) return;

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;
        m_StartEndTime = m_EndTime;
        AfflictionManager.GetAfflictionManagerInstance().Add(this);

        if (GetAfflictionType() != "Buff")
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour(GetAfflictionType()));
        }
    }

    public void ApplyRemedy(FirstAidItem fai)
    {
        UpdateRemedyItems(ref m_RemedyItems, fai.name);
        UpdateRemedyItems(ref m_AltRemedyItems, fai.name);

        if (!NeedsRemedy() && m_InstantHeal) // If you've taken all the remedy items, and it's set to cure after taking them, cure the affliction.
            Cure();
        else
            CureSymptoms(); // Otherwise, just cure the symptoms (this can be empty and do nothing)
    }
    
    public void Cure()
    {
        OnCure();
        AfflictionManager.GetAfflictionManagerInstance().Remove(this);
        PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.GetAfflictionColour("Buff"));
        InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
    }

    /// <summary>
    /// Called when InstantHeal is false and all remedy items have been taken. Can be used to run any custom code for that use case.
    /// </summary>
    protected abstract void CureSymptoms();

    public string GetAfflictionType() => HasAfflictionRisk() ? "Risk" : IsBuff() ? "Buff" : "Bad";

    public string GetSpriteName() => m_SpriteName;

    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);

    public bool HasAfflictionRisk() => m_Risk;

    private bool IsBuff() => m_Buff;
    
    /// <summary>
    /// Checks to see if the affliction needs any remedy items to be taken or not. 
    /// </summary>
    /// <returns></returns>
    public bool NeedsRemedy() => m_RemedyItems.Length > 0 && m_RemedyItems.Concat(m_AltRemedyItems).Any(item => item.Item3 > 0);

    /// <summary>
    /// Called when the affliction is cured. Can be used to run custom code for this use case.
    /// </summary>
    protected abstract void OnCure();

    /// <summary>
    /// Called when the affliction is updated. Can be used to run custom code for this use case.
    /// </summary>
    public abstract void OnUpdate();
    
    /// <summary>
    /// Checks to see if current affliction has a given item as an item to cure the affliction with.
    /// </summary>
    /// <param name="fai"></param>
    /// <returns></returns>
    public bool RequiresRemedyItem(FirstAidItem fai) => m_RemedyItems.Length > 0 && m_RemedyItems.Concat(m_AltRemedyItems).Any(item => item.Item1 == fai.m_GearItem.name);  
   
    /// <summary>
    /// Resets the entire affliction back to it's default, including remedy items and the duration.
    /// </summary>
    public void ResetAffliction()
    {
        if(m_RemedyItems.Length > 0) ResetRemedyItems(ref m_RemedyItems);
        if(m_AltRemedyItems.Length > 0) ResetRemedyItems(ref m_AltRemedyItems);

        m_EndTime = m_StartEndTime;
    }
    
    /// <summary>
    /// Used to set the given list of remedy items back to their defaults.
    /// </summary>
    /// <param name="remedyItems"></param>
    private static void ResetRemedyItems(ref Tuple<string, int, int>[] remedyItems) => remedyItems = remedyItems.Select(item => item.Item3 == 0 ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item2) : item).ToArray();
    
    private static void UpdateRemedyItems(ref Tuple<string, int, int>[] remedyItems, string itemName) => remedyItems = remedyItems.Select(item => item.Item1 == itemName ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item3 - 1) : item).ToArray();
}