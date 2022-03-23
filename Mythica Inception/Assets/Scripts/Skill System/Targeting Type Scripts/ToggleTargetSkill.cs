using System.Collections.Generic;
using System.Linq;
using _Core.Others;

namespace Skill_System.Targeting_Type_Scripts
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
                    entity.GetStateController().player.skillManager.TargetDone(slot);
                    slot.skillState = SkillManager.SkillState.active;
                }

                break;
            }
        }
        public override void DoneTargeting(IEntity entity) { }
    }
}