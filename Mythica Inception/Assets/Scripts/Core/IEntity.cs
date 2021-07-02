using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IEntity
    {
        StateController GetStateController();

        Animator GetEntityAnimator();
    }
}