namespace Assets.Scripts._Core.Player
{
    public interface IHaveStamina
    {
        void TakeStamina(int staminaToTake);
        void AddStamina(int staminaToAdd);
    }
}