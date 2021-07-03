namespace Assets.Scripts.Core
{
    public interface IHaveHealth
    {
        void TakeDamage(int damageToTake);
        void Heal(int amountToHeal);
    }
}