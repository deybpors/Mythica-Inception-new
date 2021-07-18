using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player;
using Assets.Scripts.Monster_System;
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

        public bool activated;
        public List<SkillSlot> skillSlots = new List<SkillSlot>();

        #region Hidden Fields

        [HideInInspector] public bool targeting;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 skillPoint;
        [HideInInspector] public StateMachineType smType;
        [HideInInspector] public IHaveMonsters haveMonsters;
        private IEntity _entity;

        #endregion
        
        public void ActivateSkillManager(IHaveMonsters hM)
        {
            haveMonsters = hM;
            InitializeMonsterSkills();
            _entity = GetComponent<IEntity>();
            smType = _entity.GetStateController().stateMachineType;
            activated = true;
        }

        private void InitializeMonsterSkills()
        {
            skillSlots.Clear();
            var currentMonsterSkillSlots = haveMonsters.GetMonsterSlots()[haveMonsters.CurrentSlotNumber()].skillSlots.ToList();
            foreach (var skillSlot in currentMonsterSkillSlots)
            {
                if(skillSlot == null) continue;
                var currentSkillSlot = new SkillSlot(skillSlot.skill,
                    skillSlot.cooldownTimer, skillSlot.skillState);
                skillSlots.Add(currentSkillSlot);
            }
        }

        void Update()
        {
            if(!activated) return;
            CheckTargetingAndOnCooldownSkills();
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

                        target = player.selectionManager.selectables.Count > 0 ? player.selectionManager.selectables[0] : null;

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
            slot.skill.DoneTargeting(_entity);
            targeting = false;

            ActivateWithTargetType(slot);
            
            if (slot.skill is ToggleTargetSkill) return;
            slot.skillState = SkillState.cooldown;
            slot.cooldownTimer = slot.skill.cooldownTime;
        }
        
        public void TargetDoneAI(SkillSlot slot, Vector3 position, Transform t)
        {
            ActivateSkillForAI(slot, position, t);
            
            if (slot.skill is ToggleTargetSkill) {return;}
            
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
            _entity.GetEntityAnimator().SetBool("Attack", true);
        }
        
        private void ActivateSkillForAI(SkillSlot slot, Vector3 position, Transform t)
        {
            Skill s = slot.skill;
            
            if (s is NoTargetSkill || s is ToggleTargetSkill)
            {
                s.Activate(_entity);
            }
            else if (s is AreaTargetSkill || s is VectorTargetSkill)
            {
                s.Activate(_entity, position);
            }
            else if (s is UnitAreaTargetSkill || s is UnitOnlyTargetSkill)
            {
                s.Activate(_entity, target);
            }
            else if (s is PointOrUnitSkill)
            {
                if (target == null)
                {
                    s.Activate(_entity, skillPoint);
                }
                else
                {
                    s.Activate(_entity, target);
                }
            }
            
            skillPoint = Vector3.zero;
            target = null;
            
            _entity.GetEntityAnimator().SetBool("Attack", true);

        }

        public void Targeting(SkillSlot slot)
        {
            if(!activated) return;
            
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

        public List<SkillSlot> GetAllSlots()
        {
            var slots = new List<SkillSlot>();
            foreach (var slot in skillSlots)
            {
                if (slot.skill != null)
                {
                    slots.Add(slot);
                }
            }

            return slots;
        }
    }

    [System.Serializable]
    public class SkillSlot
    {
        public Skill skill;
        public float cooldownTimer;
        public SkillManager.SkillState skillState;

        public SkillSlot(Skill skill, float cooldownTimer, SkillManager.SkillState skillState)
        {
            this.skill = skill;
            this.cooldownTimer = cooldownTimer;
            this.skillState = skillState;
        }
    }
}