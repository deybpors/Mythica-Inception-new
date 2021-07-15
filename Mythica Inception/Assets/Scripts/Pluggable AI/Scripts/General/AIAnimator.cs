using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    public class AIAnimator : MonoBehaviour
    {
        public MonsterTamerAI monsterTamerAi;
        public float magnitude;
        
        void Update()
        {
            if(monsterTamerAi.currentAnimator == null) return;

            if (monsterTamerAi.agent.velocity.magnitude > magnitude)
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
