using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class AIAnimator : MonoBehaviour
    {
        public AI _ai;
        private StateController _controller;
        

        void Update()
        {
            if (_ai.agent.velocity.magnitude > 5f)
            {
                _ai.animator.SetBool("Move", true);
                _ai.animator.SetBool("Idle", false);
            }
            else
            {
                _ai.animator.SetBool("Move", false);
                _ai.animator.SetBool("Idle", true);
            }
            //TODO: Update AI Monster for attacks, etc.
        }
    }
}
