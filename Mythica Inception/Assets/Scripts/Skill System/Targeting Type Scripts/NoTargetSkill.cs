using System.Collections.Generic;
using System.Linq;
using _Core.Others;

namespace Skill_System.Targeting_Type_Scripts
{
    public abstract class NoTargetSkill : Skill
    {
        public float radius;
        public abstract override void Activate(IEntity entity);

        public override void Target(IEntity entity)
        {
            List<SkillSlot> skillSlots = entity.GetStateController().player.skillManager.skillSlots;

            foreach (var slot in skillSlots.Where(slot => slot.skill is NoTargetSkill))
            {
                entity.GetStateController().player.skillManager.TargetDone(slot);
                break;
            }
        }
        public override void DoneTargeting(IEntity entity) { }
    }
}