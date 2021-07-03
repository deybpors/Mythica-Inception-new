using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    public class AIAnimator : MonoBehaviour
    {
        public MonsterTamerAI monsterTamerAi;
        
        void Update()
        {
            if (monsterTamerAi.agent.velocity.magnitude > 5f)
            {
                monsterTamerAi.animator.SetBool("Move", true);
                monsterTamerAi.animator.SetBool("Idle", false);
            }
            else
            {
                monsterTamerAi.animator.SetBool("Move", false);
                monsterTamerAi.animator.SetBool("Idle", true);
            }
            //TODO: Update AI Monster for attacks, etc.
        }
    }
}
