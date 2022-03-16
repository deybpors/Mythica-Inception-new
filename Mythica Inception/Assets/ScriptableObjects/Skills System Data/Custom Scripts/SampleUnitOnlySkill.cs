using _Core.Others;
using Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Skill_System.Skills.Custom_Scripts
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