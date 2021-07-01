using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Player FSM/Actions/Skill Execution")]
    public class PlayerSkillAction : Action
    {
        public override void Act(StateController stateController)
        {
            ExecuteSkill(stateController);
        }

        private void ExecuteSkill(StateController stateController)
        {
            if (stateController.player.skillManager.targeting)
            {
                //TODO: display message for player still targeting
                MakeAllSkillInputFalse(stateController);
                return;
            }
            
            if (stateController.player.inputHandler.firstSkillInput)
            {
                stateController.player.inputHandler.firstSkillInput = false;
                PickSkill(stateController, 0);
            }
            else if (stateController.player.inputHandler.secondSkillInput)
            {
                stateController.player.inputHandler.secondSkillInput = false;
                PickSkill(stateController, 1);
            }
            else if (stateController.player.inputHandler.thirdSkillInput)
            {
                stateController.player.inputHandler.thirdSkillInput = false;
                PickSkill(stateController, 2);
            }
            else if (stateController.player.inputHandler.fourthSkillInput)
            {
                stateController.player.inputHandler.fourthSkillInput = false;
                PickSkill(stateController, 3);
            }
        }

        private static void MakeAllSkillInputFalse(StateController stateController)
        {
            stateController.player.inputHandler.firstSkillInput = false;
            stateController.player.inputHandler.secondSkillInput = false;
            stateController.player.inputHandler.thirdSkillInput = false;
            stateController.player.inputHandler.fourthSkillInput = false;
        }

        private void PickSkill(StateController stateController, int slotNum)
        {
            SkillSlot skillSlot = stateController.player.skillManager.skillSlots[slotNum];
            
            if (skillSlot == null)
            {
                //TODO: display message for no skill currently in slot
                return;
            }
            
            stateController.player.skillManager.Targeting(skillSlot);
        }
    }
}