namespace AfflictionComponent.Components;

public abstract class CustomAffliction
{
    public string m_AfflictionKey;
    public string m_Cause;
    public string m_Desc;
    public string m_NoHealDesc; 

    public AfflictionBodyArea m_Location;
    public string m_SpriteName;

    public bool m_Risk;
    public bool m_Buff;

    public float m_EndTime;

    public float m_Duration; //in hours
    public bool m_Permanent;
    public bool m_InstantHeal;

    //gear item, required amount, current amount
    public Tuple<string, int, int>[] m_RemedyItems;
    public Tuple<string, int, int>[] m_AltRemedyItems;



    public bool m_BloodLoss; //the affliction causes blood loss, might not use, intended to override the vanilla Blood Loss affliction

    public CustomAffliction(string cause, string desc, string noHealDesc, AfflictionBodyArea location, string spriteName, string afflictionName, bool risk, bool buff, float duration, bool permanent, bool instantHeal, Tuple<string, int, int>[] remedyItems, Tuple<string, int, int>[] altRemedyItems)
    {
        m_Cause = cause; 
        m_Desc = desc;
        m_NoHealDesc = m_NoHealDesc;
        m_Location = location;
        m_SpriteName = spriteName;
        m_AfflictionKey = afflictionName;
        m_Risk = risk;
        m_Buff = buff;
        m_Duration = duration;
        m_Permanent = permanent;
        m_InstantHeal = instantHeal;
        m_RemedyItems = remedyItems;
        m_AltRemedyItems = altRemedyItems;

        //you can't have alternate remedy items if the main remedy items is blank
        if (m_AltRemedyItems.Length > 0 && m_RemedyItems.Length == 0)
        {
            m_RemedyItems = m_AltRemedyItems;
            m_AltRemedyItems = [];
        }
        if (m_Buff)
        {
            if(m_Risk) m_Risk = false; //buff takes precedence over risk if incorrectly assigned
            //buffs also can't have remedy items
            if (m_RemedyItems.Length > 0) m_RemedyItems = [];
            if (m_AltRemedyItems.Length > 0) m_AltRemedyItems = [];
        }

        if (m_Permanent)
        {
            m_Duration = float.PositiveInfinity;
        }
    }
    
    public void Cure()
    {
        OnCure();
        AfflictionManager.GetAfflictionManagerInstance().Remove(this);
        // Invoking the UI element on right side of the players HUD.
        PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.GetAfflictionColour("Buff"));
        InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
    }

    //called when applying all remedies and m_InstantHeal = false.
    //Users of this function are expected to implement their own logic to reset the remedy counts over time, Improved Afflictions will use this functionality
    public abstract void CureSymptoms();

    //called when curing the affliction, to trigger any logic that is needed (i.e. applying a new affliction in case of a risk)
    public abstract void OnCure();
    public string GetAfflictionType()
    {
        if (HasAfflictionRisk()) return "Risk";
        return IsBuff() ? "Buff" : "Bad";
    }
    
    public bool RequiresRemedyItem(FirstAidItem fai)
    {
        if(m_RemedyItems.Length == 0) return false;

        string gi = fai.m_GearItem.name;

        Tuple<string, int, int>[] allRemedies = m_RemedyItems.Concat(m_AltRemedyItems).ToArray();

        var elements = allRemedies.Where(i => i.Item1 == gi).ToList();
        
        return elements.Count > 0 ? true : false;
    }

    public bool NeedsRemedy()
    {
        if (m_RemedyItems.Length == 0) return false;

        Tuple<string, int, int>[] allRemedies = m_RemedyItems.Concat(m_AltRemedyItems).ToArray();

        var elements = m_RemedyItems.Where(i => i.Item3 > 0).ToList();
        var elements2 = m_AltRemedyItems.Where(i => i.Item3 > 0).ToList();

        return elements.Count <= 0 && elements2.Count <= 0 ? false : true;
    }
    public void ApplyRemedy(FirstAidItem fai)
    {
        Mod.Logger.Log("Applying remedy...", ComplexLogger.FlaggedLoggingLevel.Debug);

        for(int i = 0; i < m_RemedyItems.Length; i++)
        {
            if (m_RemedyItems[i].Item1 == fai.name)
            {
                m_RemedyItems[i] = Tuple.Create(m_RemedyItems[i].Item1, m_RemedyItems[i].Item2, m_RemedyItems[i].Item3 - 1);
            }
        }

        for (int j = 0; j < m_AltRemedyItems.Length; j++)
        {
            if (m_AltRemedyItems[j].Item1 == fai.name)
            {
                m_AltRemedyItems[j] = Tuple.Create(m_AltRemedyItems[j].Item1, m_AltRemedyItems[j].Item2, m_AltRemedyItems[j].Item3 - 1);
            }
        }

        //if you've taken all the remedy items and it's set to cure after taking them, cure the affliction
        if (!NeedsRemedy() && m_InstantHeal) Cure();
        else CureSymptoms(); //otherwise, just cure the symptoms (this can be empty and do nothing)

    }
    public string GetSpriteName() => m_SpriteName;
    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);    
    public bool HasAfflictionRisk() => m_Risk;
    private bool IsBuff() => m_Buff;
    public void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God) return; // Not quite sure if we need this here anymore as I'm curing all the afflictions when the player is in godmode.

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;

        AfflictionManager.GetAfflictionManagerInstance().Add(this); //am I allowed to do this? // You should be? Not quite sure ¯\_(ツ)_/¯

        //invoke UI element on right side of screen
        if (HasAfflictionRisk()) PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour("Risk"));
        else if (!m_Buff) PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour("Bad"));
    }
    
    //for specific events that need to occur on update
    public abstract void OnUpdate();
}