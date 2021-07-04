using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfter : MonoBehaviour
{
    public float secsAfterDisable;
    void OnEnable()
    {
        StartCoroutine(Disable(secsAfterDisable));
    }

    IEnumerator Disable(float secs)
    {
        yield return new WaitForSeconds(secs);
        gameObject.SetActive(false);
    }
}
