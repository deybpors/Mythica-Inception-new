using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts._Core
{
    public class SelectionManager : MonoBehaviour
    {
        public List<Transform> selectables;
        public Vector3 selectablePosition;
        public LayerMask layer;
        private Player.Player _player;
        private bool _activated;
    
        public void ActivateSelectionManager(Player.Player player)
        {
            _player = player;
            _activated = true;
        }

        void FixedUpdate()
        {
            if(!_activated) return;
            GetSelection();
        }
    
        private void GetSelection()
        {
            selectables.Clear();
            var ray = _player.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var hits = Physics.SphereCastAll(ray,1f, Mathf.Infinity);
            
            foreach (var hit in hits)
            {
                var selectable = hit.transform.GetComponent<ISelectable>();
                if (selectable == null) continue;
                    
                selectables.Add(hit.transform);
            }

            if (selectables.Count <= 0)
            {
                Physics.Raycast(ray, out var hit, Mathf.Infinity, layer);
                selectablePosition = hit.point;
            }
            else
            {
                selectablePosition = selectables[0].position;
            }
            
        }
    }
}
