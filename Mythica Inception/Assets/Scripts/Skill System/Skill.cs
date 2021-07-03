using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
using UnityEngine;

namespace Assets.Scripts.Skill_System
{
    public enum SkillCategory
    {
        Physical,
        Special
    }
    public abstract class Skill : ScriptableObjectWithID
    {
        public string skillName = "Skill name";
        public MonsterType skillType;
        public SkillCategory skillCategory;
        public float power = 10;
        public float cooldownTime = 10;
        public float staminaTake;
        public float healthTake;
        //TODO: change with below with id from the database to spawn in pooler
        public GameObject spawnedProjectile;

        public virtual void Activate(IEntity entity, Transform target) { }
        
        public virtual void Activate(IEntity entity) { }
        
        public virtual void Activate(IEntity entity, Vector3 position) { }

        public abstract void Target(IEntity entity);
        public abstract void DoneTargeting(IEntity entity);
        protected virtual void Deactivate(IEntity entity) { }
        
    }
}