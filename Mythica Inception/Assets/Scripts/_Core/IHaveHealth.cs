namespace Assets.Scripts._Core
{
    public interface IHaveHealth
    {
        void TakeDamage(int damageToTake);
        void Heal(int amountToHeal);

        void Die();
    }
}