using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController stateController);
    }
}
