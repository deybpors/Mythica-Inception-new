using _Core.Managers;
using UnityEngine;

namespace UI
{
    public class MinimapCam : MonoBehaviour
    {
        private Transform _target;

        private void OnEnable()
        {
            if (GameManager.instance == null) return;
            if(GameManager.instance.player == null) return;
            
            _target = GameManager.instance.player.transform;
        }

        void Update()
        {
            if (_target == null)
            {
                if(GameManager.instance.player == null) return;
                var t = GameManager.instance.player.transform;
                if (t == null)
                {
                    return;
                }
                _target = t;
            }

            var mapTransform = transform;
            var targetPosition = _target.position;
            mapTransform.position = new Vector3(targetPosition.x, mapTransform.position.y, targetPosition.z);
        }
    }
}