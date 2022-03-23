using _Core.Others;
using Combat_System;
using Skill_System;
using UnityEngine;

namespace Monster_System
{
    [CreateAssetMenu(menuName = "Monster System/Tame Beam")]
    public class TameBeam : ScriptableObjectWithID
    {
        public Skill skill;
        public ProjectileGraphics projectileGraphics;
    }
}