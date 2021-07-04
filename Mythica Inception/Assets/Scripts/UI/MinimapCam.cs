using UnityEngine;

namespace Assets.Scripts.UI
{
    public class MinimapCam : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
    }
}
