namespace AfflictionComponent.Enums;

public enum InstanceType
{
    Open, // Basically the default / not having the interface at all
    Single, // Affliction can only exist once
    SingleLocation // Affliction can only exist once per body area
}