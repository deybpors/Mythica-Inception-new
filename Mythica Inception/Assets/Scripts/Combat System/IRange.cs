using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    public interface IRange
    {
        void SetDataForProjectile(bool isTameBeam, bool canDamage, int whatValue, Transform whoSpawned, Transform whatTarget, Vector3 wherePosition, float secondsToDie, float howFast, float whatRadius);
    }
}