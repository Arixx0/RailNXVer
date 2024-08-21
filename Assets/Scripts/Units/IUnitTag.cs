namespace Units
{
    [System.Flags]
    public enum UnitTag
    {
        None = 0x000000000000,
        Train = 0x000000000001,
        EngineComp = 0x000000000010,
        CombatComp = 0x000000000100,
        ScrapperComp = 0x000000001000,
        UtilityComp = 0x000000010000,
    }
}