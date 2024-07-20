namespace AfflictionComponent.Afflictions;

internal class CustomAffliction
{
    public string m_Cause;
    public AfflictionBodyArea m_Location;
    public string m_SpriteName;

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

        if (m_Buff && m_Risk) //buff takes precedence
        {
            m_Risk = false;
        }

        if (m_Permanent)
        {
            m_Duration = float.PositiveInfinity;
        }
    }

    public void Start()
    {
        m_EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + m_Duration;

        //find component and add to list there
        //invoke UI element on right side of screen
    }

    public void Cure()
    {
        //find component and remove from list there
        //invoke UI element on right side of screen
    }

    //for specific events that need to occur on update
    public virtual void OnUpdate()
    {

    }

    //to be used in UI RefreshRightPage
    public float GetTimeRemaining()
    {
        return Mathf.CeilToInt(m_Duration * 60f);
    }

    public string GetSpriteName()
    {
        return m_SpriteName;
    }
}