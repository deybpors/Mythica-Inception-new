namespace _Core.Others
{
    public interface IHaveStamina
    {
        void TakeStamina(int staminaToTake);
        void AddStamina(int staminaToAdd);
    }
}