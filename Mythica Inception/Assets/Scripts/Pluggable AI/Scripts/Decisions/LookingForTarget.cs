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
            RemoveFromEnemiesSeePlayer(stateController);
            return true;
        }

        private void RemoveFromEnemiesSeePlayer(StateController stateController)
        {
            var enemyCount = GameManager.instance.enemiesSeePlayer.Count;
            
            for (var i = 0; i < enemyCount; i++)
            {
                var enemy = GameManager.instance.enemiesSeePlayer[i];
                if (enemy != stateController.transform) continue;
                
                GameManager.instance.enemiesSeePlayer.Remove(enemy);
                enemyCount--;
                break;
            }

            if (enemyCount != 0) return;
            
            //whenever we escaped an encounter, change the values of the data needed for the parameters
            GameManager.instance.DifficultyUpdateAdd("Failed Encounters",1);
            var player = GameManager.instance.player;
            GameManager.instance.DifficultyUpdateChange("Average Party Level", GameCalculations.MonstersAvgLevel(player.monsterSlots));
        }
    }
}
