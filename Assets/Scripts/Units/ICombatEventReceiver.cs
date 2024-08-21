using Data;

namespace Units
{
    public interface ICombatEventReceiver
    {
        public void TakeDamage(UnitCombatStatCaptureData data);
    }
}