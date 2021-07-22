using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace _Core.Others
{
    public interface IEntity
    {
        StateController GetStateController();

        Animator GetEntityAnimator();
    }
}