using System.Collections.Generic;
using _Core.Others;
using Dialogue_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Core.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public List<Transform> selectables;
        public List<IInteractable> interactables = new List<IInteractable>();
        public Vector3 selectablePosition;
        public LayerMask layer;
        private Player.Player _player;
        private bool _activated;
        private RaycastHit[] _hits = new RaycastHit[5];
        [HideInInspector] public Ray ray;
        public void ActivateSelectionManager(Player.Player player)
        {
            _player = player;
            _activated = true;
        }

        void Update()
        {
            if(!_activated) return;
            GetSelection();
        }
    
        private void GetSelection()
        {
            selectables.Clear();
            selectables.TrimExcess();
            interactables.Clear();
            interactables.TrimExcess();

            ray = _player.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var size = Physics.SphereCastNonAlloc(ray, 1f, _hits, Mathf.Infinity);

            for (var i = 0; i < size; i++)
            {
                var hit = _hits[i];
                var hitTransform = hit.transform;
                
                if (hitTransform == null) continue;

                var selectable = hitTransform.GetComponent<ISelectable>();
                var interactable = hitTransform.GetComponent<IInteractable>();

                if (selectable != null) selectables.Add(hitTransform);

                if (interactable == null) continue;
                interactables.Add(interactable);
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

        public void Select()
        {
            if (interactables.Count <= 0) return;
            if (GameManager.instance.enemiesSeePlayer.Count > 0)
            {
                var message = "You must not be in battle";
                Debug.Log(message);
                GameManager.instance.uiManager.debugConsole.DisplayLogUI(message);
                return;
            }
            
            interactables[0].Interact(_player);
        }
    }
}
