using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    public class FieldOfView : MonoBehaviour
    {
        public float viewRadius;
        [Range(0,360)]
        public float viewAngle;

        public float reactionTime = .2f;

        public LayerMask targetLayer;
        public LayerMask obstaclesLayer;
        
        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();

        void Start()
        {
            StartCoroutine("FindTargetsWithDelay", reactionTime);
        }
        

        IEnumerator FindTargetsWithDelay(float reaction)
        {
            while (true)
            {
                yield return new WaitForSeconds(reaction);
                FindVisibleTargets();
            }
        }
        
        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle/2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaclesLayer))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
            
        }
    }
}
