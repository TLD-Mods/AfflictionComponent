﻿namespace AfflictionComponent.Afflictions;

public class CustomAffliction
{
    public string m_Cause;
    public AfflictionBodyArea m_Location;
    public string m_SpriteName;
    public string m_AfflictionName;

    public bool m_Active;
    public bool m_Risk;
    public bool m_Buff;

    public float m_Duration; //in hours
    public float m_EndTime;
    public bool m_Permanent;
    public bool m_InstantHeal;
    
    public GearItem[] m_RemedyItems;

    public CustomAffliction(string cause, AfflictionBodyArea location, string spriteName, bool risk, bool buff, float duration, bool permanent, bool instantHeal, GearItem[] remedyItems)
    {
        m_Cause = cause;
        m_Location = location;
        m_SpriteName = spriteName;
        m_Risk = risk;
        m_Buff = buff;
        m_Duration = duration;
        m_Permanent = permanent;
        m_InstantHeal = instantHeal;
        m_RemedyItems = remedyItems;

        if (m_Buff && m_Risk) m_Risk = false; //buff takes precedence
        if (m_Permanent) m_Duration = float.PositiveInfinity;
    }

    public void Cure()
    {
        //find component and remove from list there
        
        // Invoking the UI element on right side of the players HUD.
        PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionName, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.AfflictionColour("Buff"));
    }
    
    public string GetSpriteName() => m_SpriteName;
    
    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);

    public bool HasAffliction() => m_Active;

    // Developers will be able to override this for their custom logic to determine if they has the affliction risk or not.
    public virtual bool HasAfflictionRisk() => false;
    
    //for specific events that need to occur on update
    public virtual void OnUpdate()
    {
        // These are just placeholders for the actual logic, again we also need to determine if it's a 'bad' affliction or not.
        if (HasAfflictionRisk())
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionName, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.AfflictionColour("Risk"));
        }
        if (HasAffliction())
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionName, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.AfflictionColour("Bad"));
        }
    }

    public void Start()
    {
        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;

        //find component and add to list there
        //invoke UI element on right side of screen
    }
}