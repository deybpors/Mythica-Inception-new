using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    public interface IRange
    {
        void ProjectileData(bool destroyOnCollide, GameObject targetFX, GameObject impactFX, 
            GameObject muzzleFX, bool isTameBeam, bool canDamage, Transform whoSpawned, 
            Transform whatTarget, Vector3 toPosition, float secondsToDie, float howFast,
            float whatRadius, Skill skill);
    }
}