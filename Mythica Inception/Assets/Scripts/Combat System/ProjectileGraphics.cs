using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    [System.Serializable]
    public class ProjectileGraphics
    {
        //TODO: change type with string to request from object pooler
        public GameObject projectile;
        public GameObject impact;
        public GameObject muzzle;
    }
}
