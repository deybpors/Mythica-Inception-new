using UnityEngine;
using UnityEngine.InputSystem;

namespace Skill_System
{
    public class AreaIndicator : MonoBehaviour
    {
        private Camera _mainCamera;
        public LayerMask layer;
        private Ray _ray;
        RaycastHit _raycastHit;
        public float radius = 1f;
        public bool updateRadius;

        void Awake()
        {
            _mainCamera = Camera.main;
            transform.localScale = new Vector3(radius, radius, radius);
        }

        void Start()
        {
            transform.localScale = new Vector3(radius, radius, radius);
        }

        void OnEnable()
        {
            transform.localScale = new Vector3(radius, radius, radius);
        }
        
        void Update()
        {
            _ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(_ray, out _raycastHit, Mathf.Infinity, layer))
            {
                transform.position = _raycastHit.point;
            }
            
            if(!updateRadius) return;
            
            transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}
