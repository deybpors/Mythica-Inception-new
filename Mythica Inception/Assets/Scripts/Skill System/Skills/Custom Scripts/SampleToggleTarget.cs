using Assets.Scripts.Core;
using Assets.Scripts.Skill_System.Targeting_Type_Scripts;
using UnityEngine;

namespace Assets.Scripts.Skill_System.Skills.Custom_Scripts
{
    [CreateAssetMenu(menuName = "Skill System/Skills/Sample Toggle Skill")]
    public class SampleToggleTarget : ToggleTargetSkill
    {

        public override void Activate(IEntity entity)
        {
            //change stats here
            Debug.Log("Activated " + skillName);
        }

        protected override void Deactivate(IEntity entity)
        {
            //change to default stats
            Debug.Log("Deactivated " + skillName);
        }
    }
}