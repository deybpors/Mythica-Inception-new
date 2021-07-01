using Assets.Scripts.Core;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Skills.Custom_Scripts
{
    [CreateAssetMenu(menuName = "Skill System/Skills/Sample Vector Target Skill")]
    public class SampleVectorTargetSkill : VectorTargetSkill
    {
        public override void Activate(IEntity entity, Vector3 target)
        {
            Debug.Log("Activate " + skillName);
        }
    }
}