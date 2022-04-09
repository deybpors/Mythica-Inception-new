using _Core.Managers;
using UnityEngine;

public class PlaySFXEnableDisable : MonoBehaviour
{

    [SerializeField] private AudioClip _audioClipOnEnable;
    [SerializeField] private AudioClip _audioClipOnDisable;

    void OnEnable()
    {
        if(_audioClipOnEnable == null) return;

        GameManager.instance.audioManager.PlaySFX(_audioClipOnEnable.name);
    }

    void OnDisable()
    {
        if (_audioClipOnEnable == null) return;

        GameManager.instance.audioManager.PlaySFX(_audioClipOnDisable.name);
    }
}
