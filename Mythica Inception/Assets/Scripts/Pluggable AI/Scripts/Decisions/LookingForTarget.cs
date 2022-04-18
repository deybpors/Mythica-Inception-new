using _Core.Managers;
using _Core.Others;
using _Core.Player;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Decisions/Looking For Target")]
    public class LookingForTarget : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return TargetNotVisible(stateController);
        }

        private bool TargetNotVisible(StateController stateController)
        {
            stateController.transform.Rotate(0, stateController.aI.aiData.searchingTurnSpeed * Time.deltaTime, 0);
            var doneSearching = stateController.HasTimeElapsed(stateController.aI.aiData.searchDuration);
            if (!doneSearching) return false;

            if (!(stateController.aI is MonsterTamerAI tamerAi)) return true;
            
            GameManager.instance.UpdateEnemiesSeePlayer(tamerAi, out var enemyCount);

            if (enemyCount > 0) return true;
            
            GameManager.instance.DifficultyUpdateAdd("Failed Encounters", 1);
            if (tamerAi.GetTameValue().currentTameValue > 0)
            {
                GameManager.instance.DifficultyUpdateAdd("Failed Tame Attempts", 1);
            }

            return true;
        }
    }
}
