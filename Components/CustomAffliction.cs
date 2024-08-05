namespace AfflictionComponent.Components;

public abstract class CustomAffliction
{
    public string m_AfflictionKey;
    public Tuple<string, int, int>[] m_AltRemedyItems;
    public bool m_BloodLoss; // The affliction causes blood loss, might not use, intended to override the vanilla Blood Loss affliction.
    public bool m_Buff;
    public string m_Cause;
    public string m_Desc;
    public float m_Duration; // In Hours.
    public float m_EndTime;
    public bool m_InstantHeal;
    public AfflictionBodyArea m_Location;
    public string m_NoHealDesc;
    public bool m_NoTimer;
    public Tuple<string, int, int>[] m_RemedyItems; // GearItem, Required Amount, Current Amount.
    public bool m_Risk;
    public string m_SpriteName;

    public CustomAffliction(string afflictionName, string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems)
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

        // You can't have alternate remedy items if the main remedy items is blank.
        if (m_AltRemedyItems.Length > 0 && m_RemedyItems.Length == 0)
        {
            m_RemedyItems = m_AltRemedyItems;
            m_AltRemedyItems = Array.Empty<Tuple<string, int, int>>();
        }

        if (m_Buff) // Buff takes precedence over risk if incorrectly assigned, they also cannot have remedy items.
        {
            m_Risk = false;
            m_RemedyItems = m_AltRemedyItems = Array.Empty<Tuple<string, int, int>>();
        }

        if (m_Risk) m_NoTimer = true;
        if (m_NoTimer) m_Duration = float.PositiveInfinity;
    }
    
    public void ApplyRemedy(FirstAidItem fai)
    {
        Mod.Logger.Log("Applying remedy...", ComplexLogger.FlaggedLoggingLevel.Debug);

        UpdateRemedyItems(m_RemedyItems, fai.name);
        UpdateRemedyItems(m_AltRemedyItems, fai.name);

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

    // Called when applying all remedies and m_InstantHeal = false.
    // Users of this function are expected to implement their own logic to reset the remedy counts over time, Improved Afflictions will use this functionality.
    public abstract void CureSymptoms();

    public string GetAfflictionType() => HasAfflictionRisk() ? "Risk" : IsBuff() ? "Buff" : "Bad";

    public string GetSpriteName() => m_SpriteName;

    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);

    public bool HasAfflictionRisk() => m_Risk;
    
    private bool IsBuff() => m_Buff;
    
    public bool NeedsRemedy() => m_RemedyItems.Length > 0 && m_RemedyItems.Concat(m_AltRemedyItems).Any(i => i.Item3 > 0);

    // Called when curing the affliction, to trigger any logic that is needed (i.e. applying a new affliction in case of a risk).
    public abstract void OnCure();

    // For specific events that need to occur on update
    public abstract void OnUpdate();
    
    public bool RequiresRemedyItem(FirstAidItem fai) => m_RemedyItems.Length > 0 && m_RemedyItems.Concat(m_AltRemedyItems).Any(i => i.Item1 == fai.m_GearItem.name);
    
    public void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God) return; // Not quite sure if we need this here anymore as I'm curing all the afflictions when the player is in god-mode.

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;
        AfflictionManager.GetAfflictionManagerInstance().Add(this); //am I allowed to do this? // You should be? Not quite sure ¯\_(ツ)_/¯.

        var afflictionColor = HasAfflictionRisk() ? "Risk" : m_Buff ? "Buff" : "Bad";
        if (afflictionColor != "Buff")
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour(afflictionColor));
        }
    }
    
    private static void UpdateRemedyItems(Tuple<string, int, int>[] remedyItems, string itemName) => _ = remedyItems.Select(item => item.Item1 == itemName ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item3 - 1) : item).ToArray();
}