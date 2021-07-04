using Assets.Scripts._Core;
using Assets.Scripts._Core.Player;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Targeting_Type_Scripts
{
    public abstract class UnitAreaTargetSkill : Skill
    {
        public float radius = 5f;

        public abstract override void Activate(IEntity entity, Transform target);

        public override void Target(IEntity entity)
        {
            Player player = entity.GetStateController().player;

            if (!player.areaIndicator.activeInHierarchy)
            {
                player.areaIndicator.GetComponent<AreaIndicator>().radius = radius;
            }

            Cursor.SetCursor(player.pointIndicator, new Vector2(player.pointIndicator.width/2, player.pointIndicator.height/2), CursorMode.Auto);
            player.areaIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.SetCursor(entity.GetStateController().player.normalCursor, Vector2.zero, CursorMode.Auto);
            entity.GetStateController().player.areaIndicator.SetActive(false);
        }
    }
}