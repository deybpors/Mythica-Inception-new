using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Skills.Custom_Scripts
{
    [CreateAssetMenu(menuName = "Skill System/Skills/Sample Unit Only Skill")]
    public class SampleUnitOnlySkill : UnitOnlyTargetSkill
    {
        public override void Activate(IEntity entity, Transform target)
        {
            Debug.Log("Activate " + skillName);
        }
    }
}