using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    public interface IMovingProjectile
    {
        void Move(bool isRange, Transform target, float velocity, float delay);
    }
}