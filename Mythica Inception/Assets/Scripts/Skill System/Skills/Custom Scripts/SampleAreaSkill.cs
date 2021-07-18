using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Skills.Custom_Scripts
{
    [CreateAssetMenu(menuName = "Skill System/Skills/Sample Area Skill")]
    public class SampleAreaSkill : AreaTargetSkill
    {
        public override void Activate(IEntity entity, Vector3 position)
        {
            Debug.Log("Activate " + skillName);
        }
    }
}