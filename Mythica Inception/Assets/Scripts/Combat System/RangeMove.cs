using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    public class RangeMove : MonoBehaviour, IMovingProjectile
    {
        public void Move(bool isRange, Transform target, float velocity, float delay)
        {
            if (!isRange) return;
            if (target != null)
            {
                var pos = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z);
                transform.position = Vector3.MoveTowards(transform.position, pos, velocity * Time.deltaTime);
            }
            else
            {
                transform.position += transform.forward * Time.deltaTime * velocity;
            }
        }
    }
}