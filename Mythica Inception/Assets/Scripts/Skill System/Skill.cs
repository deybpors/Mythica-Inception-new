using Assets.Scripts._Core;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Monster_System;
using MyBox;
using UnityEngine;

namespace Assets.Scripts.Skill_System
{
    public enum SkillCategory
    {
        Physical,
        Special,
    }
    public abstract class Skill : ScriptableObjectWithID
    {
        public bool tameBeam;
        public string skillName = "Skill name";
        [ConditionalField(nameof(tameBeam), true)] public MonsterType skillType;
        [ConditionalField(nameof(tameBeam), true)] public SkillCategory skillCategory;
        public int power = 10;
        [ConditionalField(nameof(tameBeam), true)] public float cooldownTime = 10;
        [ConditionalField(nameof(tameBeam), true)] public float castRadius;
        [ConditionalField(nameof(tameBeam), true)] public float staminaTake;
        [ConditionalField(nameof(tameBeam), true)] public float healthTake;
        
        //remember, tag of projectile in pooler is its .name
        [ConditionalField(nameof(tameBeam), true)]
        public ProjectileGraphics spawnedProjectile;
        [TextArea(15,20)] public string description;
        
        public virtual void Activate(IEntity entity, Transform target) { }
        
        public virtual void Activate(IEntity entity) { }
        
        public virtual void Activate(IEntity entity, Vector3 position) { }

        public abstract void Target(IEntity entity);
        public abstract void DoneTargeting(IEntity entity);
        protected virtual void Deactivate(IEntity entity) { }
        
    }
}