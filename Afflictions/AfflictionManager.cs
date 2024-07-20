namespace AfflictionComponent.Afflictions;

[RegisterTypeInIl2Cpp(false)]
internal class AfflictionManager : MonoBehaviour
{
    public List<CustomAffliction> m_Afflictions = [];

    public void Start() { }

    public void Update()
    {
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

    //so mod authors can check if the player has at least one CustomAffliction of their own type
    public bool HasAfflictionOfType(Type typeName)
    {
        return m_Afflictions.Any(obj => typeName.IsAssignableFrom(obj.GetType()));
    }

    public int GetCustomAfflictionCount()
    {
        return m_Afflictions.Count();
    }
}