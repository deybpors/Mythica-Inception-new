using _Core.Managers;
using UnityEngine;

namespace UI
{
    public class BillboardUI : MonoBehaviour
    {
        private Transform _cameraTransform;

        void Start()
        {
            _cameraTransform = GameManager.instance.currentWorldCamera.transform;
        }
        
        void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.forward);
        }
    }
}