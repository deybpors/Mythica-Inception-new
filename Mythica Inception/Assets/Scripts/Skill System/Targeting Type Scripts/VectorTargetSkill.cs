using _Core.Managers;
using _Core.Others;
using _Core.Player;
using UnityEngine;

namespace Skill_System.Targeting_Type_Scripts
{
    public abstract class VectorTargetSkill : Skill
    {
        public float width = 2f;
        public abstract override void Activate(IEntity entity, Vector3 target);

        public override void Target(IEntity entity)
        {
            var player = entity.GetStateController().player;
            entity.GetStateController().player.vectorIndicator.GetComponent<VectorSkillIndicator>().width = width;
            var pointIndicator = GameManager.instance.uiManager.pointIndicator;
            Cursor.SetCursor(pointIndicator, new Vector2(pointIndicator.width/2, pointIndicator.height/2), CursorMode.Auto);
            player.vectorIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.SetCursor(GameManager.instance.uiManager.normalCursor, Vector2.zero, CursorMode.Auto);
            entity.GetStateController().player.vectorIndicator.GetComponent<VectorSkillIndicator>().width = 0;
            entity.GetStateController().player.vectorIndicator.SetActive(false);
        }
    }
}