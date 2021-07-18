using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    //TODO: change to inherit item class soon
    [CreateAssetMenu(menuName = "Monster System/Tame Beam")]
    public class TameBeam : ScriptableObjectWithID
    {
        public Skill skill;
        public ProjectileGraphics projectileGraphics;
    }
}