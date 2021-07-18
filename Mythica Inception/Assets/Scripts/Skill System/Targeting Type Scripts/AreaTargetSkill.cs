using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Targeting_Type_Scripts
{
    public abstract class AreaTargetSkill : Skill
    {
        public float radius = 5f;
        public abstract override void Activate(IEntity entity, Vector3 position);

        public override void Target(IEntity entity)
        {
            Player player = entity.GetStateController().player;
            
            if(player == null || !player.skillManager.enabled) return;
            
            if (!player.areaIndicator.activeInHierarchy)
            {
                player.areaIndicator.GetComponent<AreaIndicator>().radius = radius;
            }
                
            Cursor.visible = false;
            player.areaIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.visible = true;
            Cursor.SetCursor(entity.GetStateController().player.normalCursor, Vector2.zero, CursorMode.Auto);
            entity.GetStateController().player.areaIndicator.SetActive(false);
        }
    }
}