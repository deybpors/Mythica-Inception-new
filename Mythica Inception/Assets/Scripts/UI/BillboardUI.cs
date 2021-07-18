using Assets.Scripts._Core;
using Assets.Scripts._Core.Managers;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class BillboardUI : MonoBehaviour
    {
        private Transform _cameraTransform;

        void Start()
        {
            _cameraTransform = GameManager.instance.mainCamera.transform;
        }
        
        void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.forward);
        }
    }
}