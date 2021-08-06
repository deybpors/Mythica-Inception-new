using _Core.Managers;
using _Core.Others;
using _Core.Player;
using UnityEngine;

namespace Skill_System.Targeting_Type_Scripts
{
    public abstract class AreaTargetSkill : Skill
    {
        public float radius = 5f;
        public abstract override void Activate(IEntity entity, Vector3 position);

        public override void Target(IEntity entity)
        {
            var player = entity.GetStateController().player;
            
            if(player == null || !player.skillManager.enabled) return;
            
            if (!GameManager.instance.uiManager.areaIndicator.activeInHierarchy)
            {
                GameManager.instance.uiManager.areaIndicator.GetComponent<AreaIndicator>().radius = radius;
            }
                
            Cursor.visible = false;
            GameManager.instance.uiManager.areaIndicator.SetActive(true);
        }

        public override void DoneTargeting(IEntity entity)
        {
            Cursor.visible = true;
            Cursor.SetCursor(GameManager.instance.uiManager.normalCursor, Vector2.zero, CursorMode.Auto);
            GameManager.instance.uiManager.areaIndicator.SetActive(false);
        }
    }
}