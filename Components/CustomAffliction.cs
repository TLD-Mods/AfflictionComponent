using AfflictionComponent.Interfaces;
using Newtonsoft.Json;

namespace AfflictionComponent.Components;

public abstract class CustomAffliction
{
    public string m_CauseText;
    public string m_Description;
    public string? m_DescriptionNoHeal;
    public AfflictionBodyArea m_Location;
    public string m_Name;
    [JsonProperty]
    private string m_SpriteName;

    // Interface fields
    /**
    public readonly IBuff InterfaceBuff = new IBuffImpl();
    public readonly IDuration InterfaceDuration = new IDurationImpl();
    public readonly IRemedies InterfaceRemedies = new IRemediesImpl();
    public readonly IRisk InterfaceRisk = new IRiskImpl();  
    public readonly IInstance InterfaceInstance = new IInstanceImpl();
    **/

    protected CustomAffliction(string name, string causeText, string description, string? descriptionNoHeal, string spriteName, AfflictionBodyArea location)
    {
        m_CauseText = Localization.Get(causeText); 
        m_Description = Localization.Get(description);
        m_DescriptionNoHeal = Localization.Get(descriptionNoHeal);
        m_Location = location;
        m_Name = Localization.Get(name);
        m_SpriteName = spriteName;
        
        // Check for implemented interfaces here, and then change certain conditionals.
        var iRisk = AfflictionManager.TryGetInterface<IRisk>(this);
            
        var iRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if (iRemedies != null)
        {
            // This seems to be causing an object no reference error - will fix later.
            /*if (InterfaceRemedies.AltRemedyItems.Length > 0 && InterfaceRemedies.RemedyItems.Length == 0) // You can't have alternate remedy items if the main remedy items is blank.
            {
                InterfaceRemedies.RemedyItems = InterfaceRemedies.AltRemedyItems;
                InterfaceRemedies.AltRemedyItems = [];
            }*/
        }
        
        var iBuff = AfflictionManager.TryGetInterface<IBuff>(this);
        if (iBuff != null)
        {            
            if (iBuff.Buff) // Buff takes precedence over risk if incorrectly assigned, they also cannot have remedy items.
            {
                if (iRisk != null) iRisk.Risk = false;
                if (iRemedies != null) iRemedies.RemedyItems = iRemedies.AltRemedyItems = [];
            }
        }
    }
    
    public void ApplyRemedy(FirstAidItem fai)
    {
        if (!ApplyRemedyCondition()) return;

        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if (InterfaceRemedies is null) return;

        UpdateRemedyItems(InterfaceRemedies, fai.name);
        UpdateAltRemedyItems(InterfaceRemedies, fai.name);

        if (!NeedsRemedy() && InterfaceRemedies.InstantHeal) // If you've taken all the remedy items, and it's set to cure after taking them, cure the affliction.
            Cure();
        else
            if (InterfaceRemedies != null) InterfaceRemedies.CureSymptoms(); // Otherwise, just cure the symptoms (this can be empty and do nothing)
    }

    /// <summary>
    /// Optional override used to define a condition when applying remedy items, if the condition is false the remedy item will not be applied when taken. Default is true.
    /// </summary>
    /// <returns></returns>
    protected virtual bool ApplyRemedyCondition() => true;
    
    public void Cure(bool displayHealed = true)
    {

        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if (InterfaceRemedies != null) InterfaceRemedies.OnCure();
        AfflictionManager.GetAfflictionManagerInstance().Remove(this);
        if (HasBuff()) displayHealed = false;
        if (displayHealed) PlayerDamageEvent.SpawnAfflictionEvent(m_Name, "GAMEPLAY_Healed", m_SpriteName, AfflictionManager.GetAfflictionColour("Buff"));
        InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
    }

    public string GetAfflictionType() => HasRisk() ? "Risk" : HasBuff() ? "Buff" : "Bad";

    private static int GetResetValue(string item, int value)
    {
        if (item == "GEAR_BottlePainKillers") return value > 1 ? value / 2 : value;
        return value;
    }
    
    public string GetSpriteName() => m_SpriteName;

    public bool HasBuff()
    {
        var InterfaceBuff = AfflictionManager.TryGetInterface<IBuff>(this);
        return InterfaceBuff is not null ? InterfaceBuff.Buff : false;
    }
    public bool HasDuration()
    {
        var InterfaceDuration = AfflictionManager.TryGetInterface<IDuration>(this);
        return InterfaceDuration is not null ? InterfaceDuration.Duration > 0 : false;
    }

    public bool HasRisk()
    {
        var InterfaceRisk = AfflictionManager.TryGetInterface<IRisk>(this);
        return InterfaceRisk is not null ? InterfaceRisk.Risk : false;
    }

    public bool HasRemedies(bool alt = false)
    {
        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if(InterfaceRemedies == null) return false;

        return alt ? InterfaceRemedies.AltRemedyItems != null && InterfaceRemedies.AltRemedyItems.Length > 0 : InterfaceRemedies.RemedyItems != null && InterfaceRemedies.RemedyItems.Length > 0;
    }

    /// <summary>
    /// Checks to see if the affliction needs any remedy items to be taken or not. 
    /// </summary>
    /// <returns></returns>
    public bool NeedsRemedy()
    {
        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if (InterfaceRemedies == null) return false;

        var remedyItems = InterfaceRemedies.RemedyItems;
        var altRemedyItems = InterfaceRemedies.AltRemedyItems;

        if (remedyItems == null && altRemedyItems == null) return false;
        if (remedyItems == null) remedyItems = [];
        if (altRemedyItems == null) altRemedyItems = [];

        return remedyItems.Length > 0 && remedyItems.Concat(altRemedyItems).Any(item => item is { Item3: > 0 });
    }

    /// <summary>
    /// Called when the affliction is updated. Can be used to run custom code for this use case.
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// Checks to see if current affliction has a given item as an item to cure the affliction with.
    /// </summary>
    /// <param name="fai"></param>
    /// <returns></returns>
    public bool RequiresRemedyItem(FirstAidItem fai)
    {
        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        if (InterfaceRemedies == null) return false;
        return InterfaceRemedies.RemedyItems.Length > 0 && InterfaceRemedies.RemedyItems.Concat(InterfaceRemedies.AltRemedyItems).Any(item => item.Item1 == fai.m_GearItem.name);
    }
    /// <summary>
    /// Resets the entire affliction back to its default, including remedy items and the duration.
    /// </summary>
    public void ResetAffliction(bool resetRemedies = true)
    {
        var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(this);
        var InterfaceDuration = AfflictionManager.TryGetInterface<IDuration>(this);

        if (resetRemedies && InterfaceRemedies != null)
        {
            if (InterfaceRemedies.RemedyItems.Length > 0) ResetRemedyItems(InterfaceRemedies);
            if (InterfaceRemedies.AltRemedyItems.Length > 0) ResetAltRemedyItems(InterfaceRemedies);
        }

        if (InterfaceDuration == null) return;

        InterfaceDuration.EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + InterfaceDuration.Duration;
    }
    
    public static void ResetAltRemedyItems(IRemedies iRemedies) => iRemedies.AltRemedyItems = iRemedies.AltRemedyItems.Select(item => item.Item3 == 0 ? new Tuple<string, int, int>(item.Item1, item.Item2, GetResetValue(item.Item1, item.Item2)) : item).ToArray();

    /// <summary>
    /// Used to set the given list of remedy items back to their defaults.
    /// </summary>
    /// <param name="iRemedies"></param>
    public static void ResetRemedyItems(IRemedies iRemedies) => iRemedies.RemedyItems = iRemedies.RemedyItems.Select(item => item.Item3 == 0 ? new Tuple<string, int, int>(item.Item1, item.Item2, GetResetValue(item.Item1, item.Item2)) : item).ToArray();
    
    public void Start()
    {
        if (GameManager.GetPlayerManagerComponent().m_God) return;

        var InterfaceInstance = AfflictionManager.TryGetInterface<IInstance>(this);
        var InterfaceDuration = AfflictionManager.TryGetInterface<IDuration>(this);

        //the below code is a bit hard to read but it gets the job done
        if (InterfaceInstance is not null)
        {
            CustomAffliction? existingAff = null;
            if (InterfaceInstance.type == Resources.InstanceType.Single) existingAff = Mod.afflictionManager.m_Afflictions.Any(aff => aff.m_Name == m_Name) ? Mod.afflictionManager.m_Afflictions.Where(aff => aff.m_Name == m_Name).ElementAt(0) : null;
            else if (InterfaceInstance.type == Resources.InstanceType.SingleLocation) existingAff = Mod.afflictionManager.m_Afflictions.Any(aff => aff.m_Name == m_Name && aff.m_Location == m_Location) ? Mod.afflictionManager.m_Afflictions.Where(aff => aff.m_Name == m_Name && aff.m_Location == m_Location).ElementAt(0) : null;
            if (existingAff != null)
            {
                InterfaceInstance.OnFoundExistingInstance(existingAff);
                return;
            }
        }

        //calling HasDuration here is a bit redundant butttt it's not a big deal
        if (HasDuration()) InterfaceDuration.EndTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() + InterfaceDuration.Duration;
        AfflictionManager.GetAfflictionManagerInstance().Add(this);

        if (HasBuff())
            InterfaceManager.GetPanel<Panel_HUD>().ShowBuffNotification(m_Name, "GAMEPLAY_BuffHeader", m_SpriteName);
        else
            PlayerDamageEvent.SpawnAfflictionEvent(m_Name, "GAMEPLAY_Affliction", m_SpriteName, AfflictionManager.GetAfflictionColour(GetAfflictionType()));
    }

    private static void UpdateAltRemedyItems(IRemedies iRemedies, string itemName) => iRemedies.AltRemedyItems = iRemedies.AltRemedyItems.Select(item => item.Item1 == itemName ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item3 - 1) : item).ToArray();
    
    private static void UpdateRemedyItems(IRemedies iRemedies, string itemName) => iRemedies.RemedyItems = iRemedies.RemedyItems.Select(item => item.Item1 == itemName ? new Tuple<string, int, int>(item.Item1, item.Item2, item.Item3 - 1) : item).ToArray();
}