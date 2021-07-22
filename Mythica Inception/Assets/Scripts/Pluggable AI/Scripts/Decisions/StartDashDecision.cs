using _Core.Managers;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Pluggable_AI.Scripts.Decisions
{
    [CreateAssetMenu(menuName = "Player FSM/Decisions/Start Dash Decision")]
    public class StartDashDecision : Decision
    {
        public override bool Decide(StateController stateController)
        {
            return StartDash(stateController);
        }

        private bool StartDash(StateController stateController)
        {
            var dashInput = stateController.player.inputHandler.dashInput;
            if (dashInput)
            {
                var dash = GameManager.instance.pooler.SpawnFromPool(null, stateController.player.dashGraphic.name,
                    stateController.player.dashGraphic, stateController.transform.position,
                    stateController.transform.rotation);
                dash.GetComponent<ParticleSystem>().Play();
            }
            return dashInput;
        }
    }
}