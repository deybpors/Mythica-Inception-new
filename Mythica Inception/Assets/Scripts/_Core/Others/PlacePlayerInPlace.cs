using _Core.Managers;
using MyBox;
using UnityEngine;

public class PlacePlayerInPlace : MonoBehaviour
{
    [SerializeField] private bool _always;
    [ConditionalField(nameof(_always), true)] [SerializeField] private string _saveKey;

    private bool _placed = false;

    void Update()
    {
        if (!_always)
        {
            GameManager.instance.saveManager.LoadDataObject(_saveKey, out bool isPlaced);
            _placed = isPlaced;
        }

        if (_placed)
        {
            gameObject.SetActive(false);
            return;
        }

        if(GameManager.instance == null) return;
        if(GameManager.instance.player == null) return;
        var playerTransform = GameManager.instance.player.transform;
        var thisTransform = transform;
        playerTransform.position = thisTransform.position;
        playerTransform.rotation = thisTransform.rotation;
        _placed = true;

        if (!_always)
        {
            GameManager.instance.saveManager.SaveOtherData(_saveKey, _placed);
        }
        
        
        gameObject.SetActive(false);
        
    }
}
