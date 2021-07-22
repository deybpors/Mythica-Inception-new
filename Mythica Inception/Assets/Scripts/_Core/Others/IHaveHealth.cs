namespace _Core.Others
{
    public interface IHaveHealth
    {
        void TakeDamage(int damageToTake);
        void Heal(int amountToHeal);

        void Die();
    }
}