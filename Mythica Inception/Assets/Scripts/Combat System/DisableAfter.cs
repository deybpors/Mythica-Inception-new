using System.Collections;
using _Core.Managers;
using UnityEngine;

namespace Combat_System
{
    public class DisableAfter : MonoBehaviour
    {
        public bool backToPoolAfterDisable;
        public float secsAfterDisable;
        void OnEnable()
        {
            StartCoroutine(Disable(secsAfterDisable));
        }

        IEnumerator Disable(float secs)
        {
            yield return new WaitForSeconds(secs);
            if (backToPoolAfterDisable)
            {
                transform.parent = GameManager.instance.transform;
            }
            gameObject.SetActive(false);
        }
    }
}
