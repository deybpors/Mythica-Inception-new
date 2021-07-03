using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
using UnityEngine;

namespace Assets.Scripts.Databases.Scripts
{
    [CreateAssetMenu(menuName = "Databases/Monsters Database")]
    public class MonsterDatabase : Database
    {
        public List<Monster> monsters;

        public override object FindInDatabase(ScriptableObjectWithID obj, string id)
        {
            return monsters.FirstOrDefault(monster => obj.ID.Equals(id));
        }
    }
}
