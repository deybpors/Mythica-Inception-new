using Assets.Scripts.Combat_System;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    //TODO: change to inherit item class soon
    [CreateAssetMenu(menuName = "Monster System/Tame Beam")]
    public class TameBeam : ScriptableObjectWithID
    {
        public int power;
        public ProjectileGraphics projectileGraphics;
    }
}