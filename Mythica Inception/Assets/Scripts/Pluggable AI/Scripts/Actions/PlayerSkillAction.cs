using _Core.Managers;
using Pluggable_AI.Scripts.General;
using Skill_System;
using UnityEngine;

namespace Pluggable_AI.Scripts.Actions
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
            if (!stateController.player.skillManager.activated)
            {
                MakeAllSkillInputFalse(stateController);
                return;
            }
            
            if (stateController.player.skillManager.targeting)
            {
                GameManager.instance.uiManager.debugConsole.DisplayLogUI("Player is still targeting.");
                MakeAllSkillInputFalse(stateController);
                return;
            }
            
            if (GameManager.instance.inputHandler.firstSkillInput)
            {
                GameManager.instance.inputHandler.firstSkillInput = false;
                PickSkill(stateController, 0);
            }
            else if (GameManager.instance.inputHandler.secondSkillInput)
            {
                GameManager.instance.inputHandler.secondSkillInput = false;
                PickSkill(stateController, 1);
            }
            else if (GameManager.instance.inputHandler.thirdSkillInput)
            {
                GameManager.instance.inputHandler.thirdSkillInput = false;
                PickSkill(stateController, 2);
            }
            else if (GameManager.instance.inputHandler.fourthSkillInput)
            {
                GameManager.instance.inputHandler.fourthSkillInput = false;
                PickSkill(stateController, 3);
            }
        }

        private static void MakeAllSkillInputFalse(StateController stateController)
        {
            GameManager.instance.inputHandler.firstSkillInput = false;
            GameManager.instance.inputHandler.secondSkillInput = false;
            GameManager.instance.inputHandler.thirdSkillInput = false;
            GameManager.instance.inputHandler.fourthSkillInput = false;
        }

        private void PickSkill(StateController stateController, int slotNum)
        {
            SkillSlot skillSlot = stateController.player.skillManager.skillSlots[slotNum];

            if (skillSlot.skill == null)
            {
                GameManager.instance.uiManager.debugConsole.DisplayLogUI("No skill currently in the slot.");
                return;
            }
            
            stateController.player.skillManager.Targeting(skillSlot);
        }
    }
}