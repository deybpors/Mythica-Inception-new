using Monster_System;

namespace _Core.Others
{
    public interface IHaveHealth : IEntity
    {
        void TakeDamage(int damageToTake);
        void Heal(int amountToHeal);

        void RecordDamager(MonsterSlot slot);
        
        void Die();
    }
}