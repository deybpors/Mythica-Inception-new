using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Databases.Scripts
{
    [CreateAssetMenu(menuName = "Databases/Skills Database")]
    public class SkillDatabase : Database
    {
        public List<Skill> skills;
        public override object FindInDatabase(ScriptableObjectWithID obj, string id)
        {
            return skills.FirstOrDefault(monster => obj.ID.Equals(id));
        }
    }
}