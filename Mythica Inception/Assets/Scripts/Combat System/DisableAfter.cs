using System.Collections;
using Assets.Scripts._Core;
using UnityEngine;

namespace Assets.Scripts.Combat_System
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
            gameObject.SetActive(false);
            if (backToPoolAfterDisable)
            {
                transform.parent = GameManager.instance.transform;
            }
        }
    }
}
