# AfflictionComponent

AfflictionComponent is a framework mod that allows modders to add their own afflictions to The Long Dark in their own mods. 

Custom afflictions are defined by the modders and when they are active, are tracked and managed by the newly added Affliction Manager component!

Modders can add afflictions of all sorts which allow the following:

* Risks
* Buffs
* Afflictions with or without a duration timer
* Afflictions with or without first aid remedy items to heal with
* Affictions with custom UI icons
* Custom behavior

## Custom Afflictions

The backbone of AfflictionComponent is the ability for modders to define their own afflictions. In order to create a new affliction, you must extend the CustomAffliction class.

Start by defining a new class which will be your custom affliction and extend the CustomAffliction class. 
There are a number of properties in the CustomAffliction class which serve as a base for the affliction.

* string m_Name - The name of the affliction
* string m_Description - The description of the affliction
* string m_DescriptionNoHeal - The description of the affliction which appears on the right side of the status screen if the affliction is not curable
* string m_Cause - The cause of the affliction
* AfflictionBodyArea m_Location - The location of the affliction on the player. This property uses the vanilla enum AfflictionBodyArea
* string m_SpriteName - The name of the UI sprite icon for the affliction
* bool m_CustomSprite - Whether the affliction uses custom UI sprites (more on that below)

There are also a few overridable methods you can implement which allow for custom behavior

* void OnUpdate() - Called by the AfflictionManager component every update. Can be used to run custom behavior every frame or make use of the behavior interfaces (see below)
* bool ApplyRemedyCondition() - If returns true, the default functionality will apply when using a valid remedy item for the affliction. If it returns false, this functionality can be overriden.

As well as many bespoke methods to take advantage of, some of which are used by the default behavior of AfflictionComponent:

* void ResetAffliction(bool ResetRemedies) - Used to reset the affliction to it's starting state including it's duration and optionally, it's remedies. 
  
Once you have created an instance of your custom affliction class, you can call the Start() method to apply it to the player.

## Behavior Interfaces

AfflictionComponent allows for more complex afflictions by implementing several interfaces.

### IDuration

The IDuration interface allows for your affliction to duress over time. It contains 2 properties to do this.

* float Duration - The time it takes for affliction to heal naturally in hours
* float EndTime - The calculated time it takes for your affliction to heal based on the Duration property

The EndTime property is calculated and set on Start() if your Duration property is set. The Duration property can be initialized in the constructor before you Start()
Modifying the Duration property after the affliction has been applied will not adjust the actual duration of the affliction.

* bool IsDurationUp() will tell you whether the affliction duration is complete or not.
* float GetTimeRemaining() will give you the duration in minutes.

Once the duration is up, the affliction will cure itself.
This interface allows for an affliction to cure over time, the duration for this is displayed in the Status screen.

### IBuff

The IBuff interface designates your affliction as a Buff affliction.

* bool Buff - A flag that when true confirms the instance of the class that implements the interface is a Buff
* bool BuffCold - A flag that designates whether your buff affects the cold status
* bool BuffFatigue - A flag that designates whether your buff affects the fatigue status
* bool BuffThirst - A flag that designates whether your buff affects the thirst status
* bool BuffHunger- A flag that designates whether your buff affects the hunger status

### IRiskPercentage

The IRiskPercentage interface extends the IRisk interface and designates your affliction as a Risk affliction.

* bool Risk -  A flag that when true confirms the instance of the class that implements the interface is a Risk

The risk percentage variable is not contained within the interface for greater flexibility, and should be instantiated in it's own float field.

The risk percentage can be implemented with the following methods

* float GetRiskValue() - Can be used to retrieve the risk percentage field.
* void UpdateRiskValue() - Can be used to update the risk percentage field based on the criteria of the affliction. Ideally called in OnUpdate()

The GetRiskValue() retrieves the risk value and reflects the change in the status page UI.

### IRemedies

The IRemedies interface can be used to give your afflictions items to heal with.

There are two properties that are used to hold remedy item information.

* Tuple<string, int, int>[] RemedyItems - An array of tuples that holds information about the main remedy items
* Tuple<string, int, int>[] AltRemedyItems - An array of tuples that holds information about alternative remedy items

Each tuple represents an item. It is recommended that both arrays don't hold more than 2 tuples, since the UI cannot support this.

The string is the full gear item name of the FirstAidItem used to heal the affliction.
The 1st integer is the total required amount of the item needed to heal
The 2nd integer is the current amount of the item remaining to take

When the correct first aid item is consumed, the 2nd integer is decremented until it hits 0, meaning all of the given item has been taken/used.

* bool InstantHeal - A flag that designates the behavior of healing the affliction. If this is set to true, once all the remedy items have been taken, the affliction will cure.
If this flag is set to false, the CureSymptoms() method will be called instead.

* void CureSymptoms() - A method used to define any custom behavior that should run when remedy items are taken but the affliction is not cured.
* void OnCure() - A method used to define any custom behavior that should run when the affliction is fully cured (also applies to when the duration is up, if that interface is implemented)

### IInstance

The IInstance interface is used to define behavior surrounding multiple instances of the affliction

A new enum has been defined for this interface. The InstanceType. It's values are as follows;

- Open - The default behavior of afflictions, multiple instances are allowed. Equivalent to not implementing this interface at all.
- Single - The affliction is only allowed to have ONE instance at any given time.
- SingleLocation - The affliction is only allowed to have one instance at any given AfflictionBodyArea at any given time.

* InstanceType Type - Used to define the behavior of instancing the affliction.

If the Type property is Single or SingleLocation, any new instance of the affliction will not be added to the AfflictionManager on Start() and will be halted.

* void OnFoundExistingInstance(CustomAffliction customAffliction) - A method that runs if new instances of the affliction are not allowed. Useful to run custom behavior for that use case.

### Special Cases using Interfaces

There are certain starting conditions to be aware of using these behavior interfaces

1. Buffs take precedence over risks. If you implement the risk interface and the buff interface and Buff is true, Risk will be set to false.
2. Buffs cannot have remedy items. If you implement the buff interface and Buff is true, the remedy items arrays will be emptied.
3. If you supply AltRemedyItems but leave RemedyItems empty, the two will be swapped since RemedyItems is the main set of healing items for the affliction.

