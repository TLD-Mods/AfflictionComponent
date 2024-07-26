namespace AfflictionComponent.Components;

public abstract class CustomAffliction
{
    public string m_Cause;
    public AfflictionBodyArea m_Location;
    public string m_SpriteName;
    public string m_AfflictionKey;

    public bool m_Risk;
    public bool m_Buff;

    public float m_EndTime;

    public float m_Duration; //in hours
    public bool m_Permanent;
    public bool m_InstantHeal;
    
    public Tuple<string, int>[] m_RemedyItems;

    public bool m_BloodLoss; //the affliction causes blood loss, might not use, intended to override the vanilla Blood Loss affliction
    
    public CustomAffliction(string cause, AfflictionBodyArea location, string spriteName, string afflictionName, bool risk, bool buff, float duration, bool permanent, bool instantHeal, Tuple<string, int>[] remedyItems)
    {
        this.m_Cause = cause; // We don't neccessarily need to add 'this.', it's redundant - but we can keep it if you'd like.
        this.m_Location = location;
        this.m_SpriteName = spriteName;
        this.m_AfflictionKey = afflictionName;
        this.m_Risk = risk;
        this.m_Buff = buff;
        this.m_Duration = duration;
        this.m_Permanent = permanent;
        this.m_InstantHeal = instantHeal;
        this.m_RemedyItems = remedyItems;

        if (this.m_Buff && this.m_Risk) this.m_Risk = false; //buff takes precedence
        if (this.m_Permanent) this.m_Duration = float.PositiveInfinity;
    }
    
    public void Cure()
    {
        AfflictionManager.GetAfflictionManagerInstance().Remove(this);
        // Invoking the UI element on right side of the players HUD.
        PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.AfflictionColour("Buff"));
    }
    
    public string GetSpriteName() => m_SpriteName;
    public float GetTimeRemaining() => Mathf.CeilToInt(m_Duration * 60f);    
    public bool HasAfflictionRisk() => m_Risk;

    public bool IsBuff() => m_Buff;

    public string GetAfflictionType()
    {
        if(HasAfflictionRisk()) return "Risk";
        else return IsBuff() ? "Buff" : "Bad";
    }

    //for specific events that need to occur on update
    public abstract void OnUpdate();
    public void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God)
        {
            return;
        }

        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;

        AfflictionManager.GetAfflictionManagerInstance().Add(this); //am I allowed to do this? // You should be? Not quite sure ¯\_(ツ)_/¯

        //invoke UI element on right side of screen
        if (HasAfflictionRisk())
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.AfflictionColour("Risk"));
        }
        else if (!m_Buff)
        {
            PlayerDamageEvent.SpawnAfflictionEvent(m_AfflictionKey, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.AfflictionColour("Bad"));
        }

    }
}