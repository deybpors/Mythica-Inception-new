using _Core.Managers;
using _Core.Others;
using _Core.Player;
using UnityEngine;

namespace Skill_System.Targeting_Type_Scripts
{
    public abstract class UnitOnlyTargetSkill : Skill
    {
        public abstract override void Activate(IEntity entity, Transform target);

        public override void Target(IEntity entity)
        {
            var player = entity.GetStateController().player;
            var pointIndicator = GameManager.instance.uiManager.pointIndicator;
            Cursor.SetCursor(pointIndicator, new Vector2(pointIndicator.width/2, pointIndicator.height/2), CursorMode.Auto);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.SetCursor(GameManager.instance.uiManager.normalCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}