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
            try
            {
                _cameraTransform = GameManager.instance.currentWorldCamera.transform;
            }
            catch
            {
                _cameraTransform = GameObject.FindObjectOfType<Camera>().transform;
            }
            _thisTransform = transform;
        }
        
        void LateUpdate()
        {
            try
            {
                _thisTransform.LookAt(_thisTransform.position + _cameraTransform.forward);
            }
            catch
            {
                //ignored
            }
        }
    }
}