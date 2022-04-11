using _Core.Managers;
using UnityEngine;

public class PlaySFXEnableDisable : MonoBehaviour
{

    [SerializeField] private string _audioOnEnable = "UI Open";
    [SerializeField] private string _audioOnDisable = "UI Close";
    private GameObject _thisObject;

    void OnEnable()
    {
        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }

        if(_audioOnEnable.Equals(string.Empty)) return;

        GameManager.instance.audioManager.PlaySFX(_audioOnEnable);
    }

    void OnDisable()
    {

        if (_audioOnDisable.Equals(string.Empty)) return;

        GameManager.instance.audioManager.PlaySFX(_audioOnDisable);
    }
}
