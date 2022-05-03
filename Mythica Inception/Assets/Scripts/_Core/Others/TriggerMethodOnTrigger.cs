using _Core.Managers;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class TriggerMethodOnTrigger : MonoBehaviour
{
    public bool triggerOnce;
    private bool _triggered;

    [ConditionalField(nameof(triggerOnce))] [SerializeField] private string _saveKey;


    public UnityEvent actions;

    void Awake()
    {
        GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool triggered);
        _triggered = triggered;
        if (_triggered)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_triggered && triggerOnce)
        {
            Destroy(gameObject);
            return;
        }

        if(other != GameManager.instance.player.playerCollider) return;
        actions?.Invoke();
        _triggered = true;

        GameManager.instance.saveManager.SaveOtherData(_saveKey, _triggered);
    }
}
