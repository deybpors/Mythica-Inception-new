using _Core.Managers;
using UnityEngine;

namespace Assets.Scripts._Core.Others
{
    public class UnstuckCheckpoints : MonoBehaviour
    {
        private Transform _thisTransform;
        private GameObject _thisObject;

        void OnEnable()
        {
            if (_thisTransform == null)
            {
                Initialize();
            }

            try
            {
                GameManager.instance.unstuckCheckpoints.Add(_thisObject, _thisTransform);
            }
            catch
            {
                //ignored
            }
        }

        void OnDisable()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            try
            {
                GameManager.instance.unstuckCheckpoints.Remove(_thisObject);
            }
            catch
            {
                //ignored
            }
        }

        void OnApplicationQuit()
        {
            Unsubscribe();
        }

        private void Initialize()
        {
            _thisTransform = transform;
            _thisObject = gameObject;
        }
    }
}
