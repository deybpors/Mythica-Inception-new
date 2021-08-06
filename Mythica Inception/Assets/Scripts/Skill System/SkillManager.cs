using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using _Core.Others;
using _Core.Player;
using Monster_System;
using Pluggable_AI.Scripts.General;
using Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Skill_System
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
        [HideInInspector] public float pointDistance;
        [HideInInspector] public StateMachineType smType;
        
        private readonly Vector3 zero = Vector3.zero;
        private IEntity _entity;
        private IHaveMonsters haveMonsters;
        private int serializedSkillCount;
        private MonsterSlot currentMonsterSlot;
        
        #endregion
        
        public void ActivateSkillManager(IHaveMonsters hM)
        {
            haveMonsters = hM;
            InitializeMonsterSkills();
            _entity = GetComponent<IEntity>();
            smType = _entity.GetStateController().stateMachineType;
            serializedSkillCount = skillSlots.Count;
            activated = true;
        }

        private void InitializeMonsterSkills()
        {
            skillSlots.Clear();
            currentMonsterSlot = haveMonsters.GetMonsterSlots()[haveMonsters.CurrentSlotNumber()];
            skillSlots = currentMonsterSlot.skillSlots.ToList();
        }

        void Update()
        {
            if(!activated) return;
            CheckTargetingAndOnCooldownSkills();
        }

        private void CheckTargetingAndOnCooldownSkills()
        {
            for (var i = 0; i < serializedSkillCount; i++)
            {
                var slot = skillSlots[i];
                if(slot == null || slot.skill == null) continue;
                
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
                        var player = _entity.GetStateController().player;
                        player.inputHandler.activateSkill = false;

                        target = player.selectionManager.selectables.Count > 0
                            ? player.selectionManager.selectables[0]
                            : null;

                        if ((slot.skill is UnitAreaTargetSkill || slot.skill is UnitOnlyTargetSkill) && target == null)
                        {
                            //TODO display message that skill needs to have target
                            continue;
                        }

                        TargetDone(slot);
                    }
                }

                if (slot.skillState != SkillState.cooldown) continue;
                
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
            var skill = slot.skill;
            var activate = pointDistance <= slot.skill.castRadius;
            switch (skill)
            {
                case NoTargetSkill _:
                case ToggleTargetSkill _:
                    break;
                case AreaTargetSkill _:
                case VectorTargetSkill _:
                    if (!activate) break;
                    skill.Activate(_entity, skillPoint);
                    break;
                case UnitAreaTargetSkill _:
                case UnitOnlyTargetSkill _:
                    if (!activate) break;
                    skill.Activate(_entity, target);
                    break;
                case PointOrUnitSkill _ when target == null:
                    if (!activate) break;
                    skill.Activate(_entity, skillPoint);
                    break;
                case PointOrUnitSkill _:
                    if (!activate) break;
                    skill.Activate(_entity, target);
                    break;
            }

            if (target != null)
            {
                var targetUnitIndicator = target.GetComponent<MonsterTamerAI>().unitIndicator;
                if (!targetUnitIndicator.activeInHierarchy)
                {
                    targetUnitIndicator.SetActive(true);
                }
            }
            
            skillPoint = zero;
            target = null;
            _entity.GetEntityAnimator().SetBool("Attack", true);
        }
        
        private void ActivateSkillForAI(SkillSlot slot, Vector3 position, Transform t)
        {
            var skill = slot.skill;
            
            switch (skill)
            {
                case NoTargetSkill _:
                case ToggleTargetSkill _:
                    skill.Activate(_entity);
                    break;
                case AreaTargetSkill _:
                case VectorTargetSkill _:
                    skill.Activate(_entity, position);
                    break;
                case UnitAreaTargetSkill _:
                case UnitOnlyTargetSkill _:
                    skill.Activate(_entity, target);
                    break;
                case PointOrUnitSkill _ when target == null:
                    skill.Activate(_entity, skillPoint);
                    break;
                case PointOrUnitSkill _:
                    skill.Activate(_entity, target);
                    break;
            }
            
            skillPoint = zero;
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
            var count = skillSlots.Count;
            for (var i = 0; i < count; i++)
            {
                var slot = skillSlots[i];
                slots.Add(slot);
            }

            return slots;
        }

        public void Deactivate()
        {
            for (var i = 0; i < serializedSkillCount; i++)
            {
                var playerSlot = GameManager.instance.player.monsterSlots[currentMonsterSlot.slotNumber].skillSlots[i];

                if (!targeting) continue;
                
                playerSlot.skill.DoneTargeting(_entity);
                targeting = false;
            }

            activated = false;
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