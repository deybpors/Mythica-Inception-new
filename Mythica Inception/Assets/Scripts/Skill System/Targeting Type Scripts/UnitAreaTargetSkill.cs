using _Core.Managers;
using _Core.Others;
using _Core.Player;
using UnityEngine;

namespace Skill_System.Targeting_Type_Scripts
{
    public abstract class UnitAreaTargetSkill : Skill
    {
        public float radius = 5f;

        public abstract override void Activate(IEntity entity, Transform target);

        public override void Target(IEntity entity)
        {
            var player = entity.GetStateController().player;

            if (!GameManager.instance.uiManager.areaIndicator.activeInHierarchy)
            {
                GameManager.instance.uiManager.areaIndicator.GetComponent<AreaIndicator>().radius = radius;
            }

            var pointIndicator = GameManager.instance.uiManager.pointIndicator;
            Cursor.SetCursor(pointIndicator, new Vector2(pointIndicator.width/2, pointIndicator.height/2), CursorMode.Auto);
            GameManager.instance.uiManager.areaIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.SetCursor(GameManager.instance.uiManager.normalCursor, Vector2.zero, CursorMode.Auto);
            GameManager.instance.uiManager.areaIndicator.SetActive(false);
        }
    }
}