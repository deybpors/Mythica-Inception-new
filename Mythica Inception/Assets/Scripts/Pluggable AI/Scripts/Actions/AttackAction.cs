using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Pluggable_AI.Scripts.States;
using Assets.Scripts.Skill_System;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Assets.Scripts.Pluggable_AI.Scripts.Actions
{
    [CreateAssetMenu(menuName = "Pluggable AI/Actions/Attack")]
    public class AttackAction : Action
    {
        public State fleeState;
        private float _timer;
        private bool _attacked = false;
        public override void Act(StateController stateController)
        {
            Attack(stateController);
        }

        private void Attack(StateController stateController)
        {
            if (_attacked)
            {
                _timer += Time.deltaTime;
                if (_timer > .5f)
                {
                    stateController.controllerAnimator.SetBool("Attack", false);
                    _attacked = false;
                    _timer = 0;
                }
            }

            if (!stateController.stateBoolVariable)
            {
                stateController.stateTimeElapsed = stateController.aI.aiData.combatDecisionEvery;
                stateController.stateBoolVariable = true;
            }

            var fieldOfView = stateController.aI.fieldOfView;
            if (fieldOfView.visibleTargets.Count <= 0) return;
            
            if (stateController.HasTimeElapsed(stateController.aI.aiData.combatDecisionEvery))
            {
                if(_timer <= 0)
                {               
                    AttackDecision(stateController);
                }
            }
        }

        private void AttackDecision(StateController stateController)
        {
            var skillManager = stateController.GetComponent<SkillManager>();
            if (!skillManager.activated) return;
            
            var monsterTamerAI = stateController.GetComponent<MonsterTamerAI>();
            var stamina = stateController.GetComponent<Stamina>();

            //get skill slots of the current monster
            var skillSlots = skillManager.GetAllSlots();

            //get the basic attack and add it to the list of skill slots
            var basicAttack =
                new SkillSlot(monsterTamerAI.monsterSlots[monsterTamerAI.currentMonster].monster.basicAttackSkill, 
                    0, SkillManager.SkillState.ready);
            skillSlots.Add(basicAttack);
            
            //create a new list of floats that stores the score of each skills
            var skillScore = new List<float>();
            foreach (var slot in skillSlots.ToList())
            {
                if (slot.skill == null)
                {
                    skillSlots.Remove(slot);
                    continue;
                }
                skillScore.Add(SkillScoreByCastDistance(slot.skill, stateController));
            }

            //multiply the skill's score to 0 if the skill's staminaTake > monster's stamina
            //multiply the skill's score to 0 if it the skill is currently on cooldown
            //multiply the skill's score to 0 if its strategy is for defense only
            for (var i = 0; i < skillScore.Count; i++)
            {
                if (skillSlots[i] == basicAttack)
                {
                    continue;
                }
                skillScore[i] *= SkillStaminaTake(skillSlots[i].skill, stamina);
                skillScore[i] *= SkillOnCooldown(skillSlots[i]);
                skillScore[i] *= SkillStrategyCheck(skillSlots[i].skill);
            }
            
            //Get the skill/s that is allowed to execute
            var skillsToExecute = new List<SkillSlot>();
            for (var i = 0; i < skillScore.Count; i++)
            {
                if (skillSlots[i] == basicAttack)
                {
                    skillsToExecute.Add(skillSlots[i]); 
                    continue;
                }
                if (skillScore[i] > 0)
                {
                    skillsToExecute.Add(skillSlots[i]);
                }
            }
            
            //if it has no skills that can be executed then let the monster flee
            if (skillsToExecute.Count <= 0)
            {
                stateController.TransitionToState(fleeState);
                return;
            }
            
            var rand = new Random();
            var skillIndex = rand.Next(0, skillsToExecute.Count);

            //if the picked skill to execute is the basic attack
            if (skillsToExecute[skillIndex] == basicAttack)
            {
                //if there is/are other skill/s to execute that is not the basic attack only
                if (skillsToExecute.Count > 1)
                {
                    var basicAttackIndex = skillIndex;
                    //find another random skill index until it finds the skill that is not the basic attack
                    while (skillIndex == basicAttackIndex)
                    {
                        skillIndex = rand.Next(0, skillsToExecute.Count);
                    }
                }
                else //if we only have the basic attack
                {
                    var dist = Vector3.Distance(stateController.transform.position, stateController.aI.target.position);
                    if (dist >= basicAttack.skill.castRadius) return;
                    
                    monsterTamerAI.ReleaseBasicAttack();
                    stateController.controllerAnimator.SetBool("Attack", true);
                    _attacked = true;
                    return;
                }
            }
            
            //perform the skill
            skillManager.TargetDoneAI(skillsToExecute[skillIndex],
                stateController.aI.target.position,
                stateController.aI.target);
            _attacked = true;
        }

        private float SkillStrategyCheck(Skill skill)
        {
            return skill.skillAiStrategy == SkillAIStrategy.Defense ? 0f : 1f;
        }

        private float SkillStaminaTake(Skill skill, Stamina stamina)
        {
            return skill.staminaTake > stamina.stamina.currentStamina ? 0f : 1f;
        }

        private float SkillOnCooldown(SkillSlot skillSlot)
        {
            return skillSlot.skillState == SkillManager.SkillState.cooldown ? 0f : 1f;
        }

        private float SkillScoreByCastDistance(Skill skill, StateController stateController)
        {
            var distanceAiPlayer = Vector3.Distance(stateController.transform.position,
                stateController.aI.target.position);

            return distanceAiPlayer > skill.castRadius ? 0f : 1f;
        }
    }
}
