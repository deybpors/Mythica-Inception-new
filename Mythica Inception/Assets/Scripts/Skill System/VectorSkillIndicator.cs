using _Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Skill_System
{
    public class VectorSkillIndicator : MonoBehaviour
    {
        public Player player;
        public LayerMask layer;
        public float width;
        private RaycastHit _raycastHit;
        private Quaternion _transRotation;
        private Ray _ray;
        private Camera _camera;
        private RectTransform _rectTransform;
        private float _distance;

        void Awake()
        {
            _camera = player.mainCamera;
            _rectTransform = GetComponent<RectTransform>();
        }
        
        void Update()
        {
            _ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            _distance = Vector3.Distance(_raycastHit.point, _rectTransform.position);

            if (!Physics.Raycast(_ray, out _raycastHit, Mathf.Infinity, layer)) return;

            _transRotation = Quaternion.LookRotation(_raycastHit.point - _rectTransform.position);
            
            _rectTransform.sizeDelta = new Vector2(width, _distance);
            
            if (_raycastHit.point.y <= player.transform.position.y)
            {
                _transRotation.eulerAngles = new Vector3(90, _transRotation.eulerAngles.y,_transRotation.eulerAngles.z);
            }
            else
            {
                _transRotation.eulerAngles = new Vector3(_transRotation.eulerAngles.x + 90, _transRotation.eulerAngles.y,_transRotation.eulerAngles.z);
            }
            
            transform.rotation = Quaternion.Lerp(_transRotation, _rectTransform.rotation, 0f);
        }
    }
}
