using _Core.Managers;
using UnityEngine;

namespace UI
{
    public class BillboardUI : MonoBehaviour
    {
        private Transform _cameraTransform;
        private Transform _thisTransform;

        void Start()
        {
            _cameraTransform = GameManager.instance.currentWorldCamera.transform;
            _thisTransform = transform;
        }
        
        void LateUpdate()
        {
            _thisTransform.LookAt(_thisTransform.position + _cameraTransform.forward);
        }
    }
}