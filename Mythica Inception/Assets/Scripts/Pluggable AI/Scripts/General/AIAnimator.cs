using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    public class AIAnimator : MonoBehaviour
    {
        public MonsterTamerAI monsterTamerAi;
        
        void Update()
        {
            if(monsterTamerAi.currentAnimator == null) return;
            
            if (monsterTamerAi.agent.velocity.magnitude > 5f)
            {
                monsterTamerAi.currentAnimator.SetBool("Move", true);
                monsterTamerAi.currentAnimator.SetBool("Idle", false);
            }
            else
            {
                monsterTamerAi.currentAnimator.SetBool("Move", false);
                monsterTamerAi.currentAnimator.SetBool("Idle", true);
            }
        }
    }
}
