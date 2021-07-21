using _Core.Managers;
using Assets.Scripts._Core;
using Assets.Scripts._Core.Managers;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Decisions
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
            var pos = new Vector3(stateController.transform.position.x, stateController.transform.position.y + 2,
                stateController.transform.position.z);
            var dashInput = stateController.player.inputHandler.dashInput;
            if (dashInput)
            {
                var dash = GameManager.instance.pooler.SpawnFromPool(null, stateController.player.dashGraphic.name,
                    stateController.player.dashGraphic, pos,
                    stateController.transform.rotation);
                dash.GetComponent<ParticleSystem>().Play();
            }
            return dashInput;
        }
    }
}