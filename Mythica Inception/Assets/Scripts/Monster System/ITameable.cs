namespace Assets.Scripts.Monster_System
{
    public interface ITameable
    {
        void AddCurrentTameValue(int tameBeamValue, IHaveMonsters tamer);
    }
}