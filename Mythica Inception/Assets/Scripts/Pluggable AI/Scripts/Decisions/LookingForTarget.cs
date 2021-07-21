using _Core.Managers;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Player;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Decisions
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

        private static void RemoveFromEnemiesSeePlayer(StateController stateController)
        {
            for (var i = 0; i < GameManager.instance.enemiesSeePlayer.Count; i++)
            {
                var enemy = GameManager.instance.enemiesSeePlayer[i];
                if (enemy == stateController.transform)
                {
                    GameManager.instance.enemiesSeePlayer.Remove(enemy);
                }
            }
        }
    }
}
