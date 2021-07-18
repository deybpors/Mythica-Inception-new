using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Targeting_Type_Scripts
{
    public abstract class VectorTargetSkill : Skill
    {
        public float width = 2f;
        public abstract override void Activate(IEntity entity, Vector3 target);

        public override void Target(IEntity entity)
        {
            Player player = entity.GetStateController().player;
            entity.GetStateController().player.vectorIndicator.GetComponent<VectorSkillIndicator>().width = width;
            Cursor.SetCursor(player.pointIndicator, new Vector2(player.pointIndicator.width/2, player.pointIndicator.height/2), CursorMode.Auto);
            player.vectorIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.SetCursor(entity.GetStateController().player.normalCursor, Vector2.zero, CursorMode.Auto);
            entity.GetStateController().player.vectorIndicator.GetComponent<VectorSkillIndicator>().width = 0;
            entity.GetStateController().player.vectorIndicator.SetActive(false);
        }
    }
}