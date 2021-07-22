using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController stateController);
        
    }
}
