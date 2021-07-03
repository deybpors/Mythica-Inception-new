using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Assets.Scripts.Core.Player;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System
{
    public class SkillManager : MonoBehaviour
    {
        public enum SkillState
        {
            ready,
            targeting,
            cooldown,
            active
        }
    
        public List<SkillSlot> skillSlots;
        
        [HideInInspector] public bool targeting;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 skillPoint;
        [HideInInspector] public StateMachineType smType;
        private IEntity _entity;

        void Start()
        {
            InitializeMonsterSkills();
            _entity = GetComponent<IEntity>();
            smType = _entity.GetStateController().stateMachineType;
        }

        private void InitializeMonsterSkills()
        {
            //get skills from monster data here
            
            //making all skills ready
            foreach (var skillSlot in skillSlots)
            {
                skillSlot.skillState = SkillState.ready;
            }
        }

        void Update()
        {
            if (smType == StateMachineType.Player)
            {
                CheckTargetingAndOnCooldownSkills();
                return;
            }
            
            //TODO: implement AI skill here
        }

        private void CheckTargetingAndOnCooldownSkills()
        {
            foreach (var slot in skillSlots)
            {
                if (slot.skillState == SkillState.targeting)
                {
                    if (_entity.GetStateController().player.inputHandler.cancelSkill)
                    {
                        _entity.GetStateController().player.inputHandler.cancelSkill = false;
                        slot.skill.DoneTargeting(_entity);
                        slot.skillState = SkillState.ready;
                        targeting = false;
                        continue;
                    }

                    if (_entity.GetStateController().player.inputHandler.activateSkill)
                    {
                        Player player = _entity.GetStateController().player;
                        player.inputHandler.activateSkill = false;

                        target = player.GetTarget();
                        
                        if ((slot.skill is UnitAreaTargetSkill || slot.skill is UnitOnlyTargetSkill) && target == null)
                        {
                            //TODO display message that skill needs to have target
                            continue;
                        }
                        
                        TargetDone(slot);
                    }
                }

                if (slot.skillState == SkillState.cooldown)
                {
                    if (slot.cooldownTimer > 0)
                    {
                        slot.cooldownTimer -= Time.deltaTime;
                    }
                    else
                    {
                        slot.cooldownTimer = 0;
                        slot.skillState = SkillState.ready;
                    }
                }
            }
        }

        public void TargetDone(SkillSlot slot)
        {
            if (smType == StateMachineType.Player)
            {
                slot.skill.DoneTargeting(_entity);
                targeting = false;
            }

            ActivateWithTargetType(slot);
            
            if (slot.skill is ToggleTargetSkill) return;
            slot.skillState = SkillState.cooldown;
            slot.cooldownTimer = slot.skill.cooldownTime;
        }

        private void ActivateWithTargetType(SkillSlot slot)
        {
            Skill s = slot.skill;
            
            if (s is NoTargetSkill || s is ToggleTargetSkill)
            {
                s.Activate(_entity);
            }
            else if (s is AreaTargetSkill || s is VectorTargetSkill)
            {
                s.Activate(_entity, skillPoint);
                transform.LookAt(skillPoint);
            }
            else if (s is UnitAreaTargetSkill || s is UnitOnlyTargetSkill)
            {
                s.Activate(_entity, target);
                transform.LookAt(target);
            }
            else if (s is PointOrUnitSkill)
            {
                if (target == null)
                {
                    s.Activate(_entity, skillPoint);
                    transform.LookAt(skillPoint);
                }
                else
                {
                    s.Activate(_entity, target);
                    transform.LookAt(target);
                }
            }

            if (target != null)
            {
                GameObject targetUnitIndicator = target.GetComponent<MonsterTamerAI>().unitIndicator;
                if (!targetUnitIndicator.activeInHierarchy)
                {
                    targetUnitIndicator.SetActive(true);
                }
            }
            
            transform.rotation = new Quaternion(0f,transform.rotation.y, 0f, transform.rotation.w);
            skillPoint = Vector3.zero;
            target = null;
            //TODO: Play attack animation here
            _entity.GetEntityAnimator().SetBool("Attack", true);
        }

        public void Targeting(SkillSlot slot)
        {
            if (slot.cooldownTimer > 0)
            {
                //TODO: update UI that skill is still in cooldown
                return;
            }
            
            slot.skill.Target(_entity);
            if (slot.skill is ToggleTargetSkill || slot.skill is NoTargetSkill) return;
            
            targeting = true;
            slot.skillState = SkillState.targeting;
        }
        
    }

    [System.Serializable]
    public class SkillSlot
    {
        public Skill skill;
        public float cooldownTimer;
        public SkillManager.SkillState skillState;
    }
}