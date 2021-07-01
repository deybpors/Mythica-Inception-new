using Assets.Scripts.Core;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Skills.Custom_Scripts
{
    [CreateAssetMenu(menuName = "Skill System/Skills/Sample No Target Skill")]
    public class SampleNoTarget : NoTargetSkill
    {
        public override void Activate(IEntity entity)
        {
            Debug.Log("Activated " + skillName);
        }
    }
}