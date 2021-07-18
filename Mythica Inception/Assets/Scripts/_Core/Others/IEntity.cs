using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts._Core.Others
{
    public interface IEntity
    {
        StateController GetStateController();

        Animator GetEntityAnimator();
    }
}