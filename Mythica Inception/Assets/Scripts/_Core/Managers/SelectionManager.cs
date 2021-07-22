using System.Collections.Generic;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Dialogue_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts._Core.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public List<Transform> selectables;
        public List<Transform> interactables;
        public Vector3 selectablePosition;
        public LayerMask layer;
        private Player.Player _player;
        private bool _activated;
        private RaycastHit[] _hits = new RaycastHit[5];
    
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
            interactables.Clear();
            var ray = _player.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var size = Physics.SphereCastNonAlloc(ray, 1f, _hits, Mathf.Infinity);

            for (var i = 0; i < size; i++)
            {
                var hit = _hits[i];
                var selectable = hit.transform.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    selectables.Add(hit.transform);
                }
                
                var interactable = hit.transform.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactables.Add(hit.transform);
                }
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
                Debug.Log("you must not be in battle");
                return;
            }
            var distance = Vector3.Distance(_player.transform.position, interactables[0].position);
            if (distance > 2f)
            {
                Debug.Log("distance: " + distance + "\nmust go closer");
                return;
            }
            interactables[0].GetComponent<IInteractable>().Interact(_player);
            _player.inputHandler.activate = false;
        }
    }
}
