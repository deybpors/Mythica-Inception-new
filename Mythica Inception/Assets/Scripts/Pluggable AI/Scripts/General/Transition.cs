using Pluggable_AI.Scripts.Decisions;
using Pluggable_AI.Scripts.States;

namespace Pluggable_AI.Scripts.General
{
    [System.Serializable]
    public class Transition
    {
        public Decision decision;
        public State successState;
        public State failState;
    }
}
