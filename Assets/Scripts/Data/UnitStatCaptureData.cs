namespace Data
{
    // Record of captured unit stat data before it is being upgraded
    public struct UnitUpgradeStatCaptureData
    {
        public float FuelEfficiency;
        public float HealthPoint;
        public float ArmorPoint;
        public float UnitSize;
    }
    
    // Record of captured unit combat stat data when the combat object attacks
    public struct UnitCombatStatCaptureData
    {
        public float AttackDamage;
        public float ArmorPierce;
    }
}