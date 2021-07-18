using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Targeting_Type_Scripts
{
    public abstract class ToggleTargetSkill : Skill
    {
        public abstract override void Activate(IEntity entity);

        public override void Target(IEntity entity)
        {
            List<SkillSlot> skillSlots = entity.GetStateController().player.skillManager.skillSlots;

            foreach (var slot in skillSlots.Where(slot => slot.skill == this))
            {
                if (slot.skillState == SkillManager.SkillState.active)
                {
                    Deactivate(entity);
                    slot.skillState = SkillManager.SkillState.ready;
                }
                else
                {
                    //TODO change UI skill slot to activated skill
                    entity.GetStateController().player.skillManager.TargetDone(slot);
                    slot.skillState = SkillManager.SkillState.active;
                }

                break;
            }
        }
        public override void DoneTargeting(IEntity entity) { }
    }
}