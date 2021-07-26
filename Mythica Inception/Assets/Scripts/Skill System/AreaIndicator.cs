using _Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Skill_System
{
    public class AreaIndicator : MonoBehaviour
    {
        public LayerMask layer;
        private Ray _ray;
        RaycastHit _raycastHit;
        public float radius = 1f;
        public bool updateRadius;

        void Awake()
        {
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
            _ray = GameManager.instance.player.selectionManager.ray;
            if (Physics.Raycast(_ray, out _raycastHit, Mathf.Infinity, layer))
            {
                transform.position = _raycastHit.point;
            }
            
            if(!updateRadius) return;
            
            transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}
