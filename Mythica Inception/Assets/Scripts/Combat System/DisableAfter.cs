using System.Collections;
using _Core.Managers;
using UnityEngine;

namespace Combat_System
{
    public class DisableAfter : MonoBehaviour
    {
        public bool backToPoolAfterDisable;
        public float secsAfterDisable;
        private Transform _gameManagerTransform;
        private Transform _thisTransform;
        private GameObject _thisGameObject;
        void OnEnable()
        {
            if (_gameManagerTransform == null)
            {
                _gameManagerTransform = GameManager.instance.transform;
                _thisTransform = transform;
                _thisGameObject = gameObject;
            }
            StartCoroutine(Disable(secsAfterDisable));
        }

        IEnumerator Disable(float secs)
        {
            yield return new WaitForSeconds(secs);
            if (backToPoolAfterDisable)
            {
                
                _thisTransform.SetParent(_gameManagerTransform);
            }

            _thisGameObject.SetActive(false);
        }
    }
}
