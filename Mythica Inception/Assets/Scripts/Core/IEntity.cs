using Assets.Scripts.Pluggable_AI.Scripts.General;

namespace Assets.Scripts.Core
{
    public interface IEntity
    {
        StateController GetStateController();
    }
}