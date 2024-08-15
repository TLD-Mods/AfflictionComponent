﻿using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Components;

// TODO: Split certain functionality off into Interfaces, which developers can then extend onto their custom afflictions.
public abstract class CustomAffliction
{
    public string m_Name;
    public Tuple<string, int, int>[] m_AltRemedyItems;
    public bool m_BloodLoss; // The affliction causes blood loss, might not use, intended to override the vanilla Blood Loss affliction. // Do we still need this?
    public string m_CauseText;
    public string m_Description;
    private float m_Duration; // In Hours.
    public float m_EndTime;
    private bool m_InstantHeal;
    public AfflictionBodyArea m_Location;
    public string m_DescriptionNoHeal; // Should we make this nullable?
    public bool m_NoTimer;
    public Tuple<string, int, int>[] m_RemedyItems; // GearItem, Required Amount, Current Amount.
    private bool m_Risk;
    private string m_SpriteName;

    // Interface fields
    internal readonly IBuff InterfaceBuff; 
    
    protected CustomAffliction(string name, string causeText, string description, string descriptionNoHeal, AfflictionBodyArea location, string spriteName, bool risk, float duration, bool noTimer, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems)
    {
        m_CauseText = Localization.Get(causeText); 
        m_Description = Localization.Get(description);
        m_DescriptionNoHeal = Localization.Get(descriptionNoHeal);
        m_Location = location;
        m_SpriteName = spriteName;
        m_Name = Localization.Get(name);
        m_Risk = risk;
        m_Duration = duration;
        m_NoTimer = noTimer;
        m_InstantHeal = instantHeal;
        m_RemedyItems = remedyItems;
        m_AltRemedyItems = altRemedyItems;
        
        // Check for implemented interfaces here, and then change certain conditionals.
        var iBuff = AfflictionManager.TryGetInterface<IBuff>(this);
        if (iBuff != null)
        {
            InterfaceBuff = iBuff;
            
            if (InterfaceBuff.Buff) // Buff takes precedence over risk if incorrectly assigned, they also cannot have remedy items.
            {
                m_Risk = false;
                m_RemedyItems = m_AltRemedyItems = [];
            }
        }
        
        if (m_AltRemedyItems.Length > 0 && m_RemedyItems.Length == 0) // You can't have alternate remedy items if the main remedy items is blank.
        {
            m_RemedyItems = m_AltRemedyItems;
            m_AltRemedyItems = [];
        }

        if (m_Risk) m_NoTimer = true;
        if (m_NoTimer) m_Duration = float.PositiveInfinity;
    }

    public void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God) return;

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;
        AfflictionManager.GetAfflictionManagerInstance().Add(this);

        if (InterfaceBuff.Buff)
            InterfaceManager.GetPanel<Panel_HUD>().ShowBuffNotification(m_Name, "GAMEPLAY_BuffHeader", m_SpriteName);
        else
            PlayerDamageEvent.SpawnAfflictionEvent(m_Name, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour(GetAfflictionType()));
    }

    public void ApplyRemedy(FirstAidItem fai)
    {
        if (!ApplyRemedyCondition()) return;

        UpdateRemedyItems(ref m_RemedyItems, fai.name);
        UpdateRemedyItems(ref m_AltRemedyItems, fai.name);

        if (!NeedsRemedy() && m_InstantHeal) // If you've taken all the remedy items, and it's set to cure after taking them, cure the affliction.
            Cure();
        else
            CureSymptoms(); // Otherwise, just cure the symptoms (this can be empty and do nothing)
    }

    /// <summary>
    /// Optional override used to define a condition when applying remedy items, if the condition is false the remedy item will not be applied when taken. Default is true.
    /// </summary>
    /// <returns></returns>
    protected virtual bool ApplyRemedyCondition() => true;
    
    public void Cure(bool displayHealed = true)
    {
        OnCure();
        AfflictionManager.GetAfflictionManagerInstance().Remove(this);
        if (displayHealed) PlayerDamageEvent.SpawnAfflictionEvent(m_Name, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.GetAfflictionColour("Buff"));
        InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
    }

    /// <summary>
    /// Called when InstantHeal is false and all remedy items have been taken. Can be used to run any custom code for that use case.
    /// </summary>
    protected abstract void CureSymptoms();

    public string GetAfflictionType() => HasAfflictionRisk() ? "Risk" : InterfaceBuff.Buff ? "Buff" : "Bad";

    private static int GetResetValue(string item, int value)
    {
        if (item == "GEAR_BottlePainKillers") return value > 1 ? value / 2 : value;
        else return value;
    }
    
    public string GetSpriteName() => m_SpriteName;

    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);

    public bool HasAfflictionRisk() => m_Risk;
    
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
    public void ResetAffliction(bool ResetRemedies = true)
    {
        if (ResetRemedies)
        {
            if (m_RemedyItems.Length > 0) ResetRemedyItems(ref m_RemedyItems);
            if (m_AltRemedyItems.Length > 0) ResetRemedyItems(ref m_AltRemedyItems);
        }

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;
    }

    /// <summary>
    /// Used to set the given list of remedy items back to their defaults.
    /// </summary>
    /// <param name="remedyItems"></param>
    public static void ResetRemedyItems(ref Tuple<string, int, int>[] remedyItems) => remedyItems = remedyItems.Select(item => item.Item3 == 0 ? new Tuple<string, int, int>(item.Item1, item.Item2, GetResetValue(item.Item1, item.Item2)) : item).ToArray();
    
    private static void UpdateRemedyItems(ref Tuple<string, int, int>[] remedyItems, string itemName) => remedyItems = remedyItems.Select(item => item.Item1 == itemName ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item3 - 1) : item).ToArray();
}