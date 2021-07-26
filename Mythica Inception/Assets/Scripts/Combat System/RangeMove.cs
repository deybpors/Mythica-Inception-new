using UnityEngine;

namespace Combat_System
{
    public class RangeMove : MonoBehaviour, IMovingProjectile
    {
        public void Move(bool isRange, Transform target, float velocity, float delay)
        {
            if (!isRange) return;
            if (target != null)
            {
                var position = target.position;
                var pos = new Vector3(position.x, position.y, position.z);
                transform.position = Vector3.MoveTowards(transform.position, pos, velocity * Time.deltaTime);
            }
            else
            {
                var objTrans = transform;
                objTrans.position += objTrans.forward * (Time.deltaTime * velocity);
            }
        }
    }
}