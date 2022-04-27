using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Dialogue_System;
using Items_and_Barter_System.Scripts;
using MyBox;
using UnityEngine;

public class ItemDrop : MonoBehaviour, IInteractable
{
    [Layer][SerializeField] private int layer;
    [SerializeField] private float _interactableDistance = 1f;
    [ReadOnly][SerializeField] private bool _isInteractable;
    
    private GameObject _currentPrefab;
    private Outline _outline;
    private ItemObject _item;
    private int _amount;
    private Player _player;
    private Transform _playerTransform;
    private Transform _thisTransform;
    private Vector3 _down = Vector3.down;
    private Vector3 _zero = Vector3.zero;
    private GameObject _thisObject;

    private Dictionary<GameObject, Outline> _outlines = new Dictionary<GameObject, Outline>();
    private Dictionary<GameObject, Transform> _transforms = new Dictionary<GameObject, Transform>();
    private Dictionary<GameObject, Quaternion> _quaternions = new Dictionary<GameObject, Quaternion>();

    void Initialize(Vector3 initPos)
    {
        if (_player == null)
        {
            _player = GameManager.instance.player;
            _thisTransform = transform;
            _playerTransform = _player.transform;
        }

        var cast = Physics.Raycast(_thisTransform.position, _down, out var hit, layer);
        if (!cast) return;

        _thisTransform.position = initPos;
        StopAllCoroutines();
        StartCoroutine(GoToPosition(hit.point,1));
        _thisTransform.rotation = Quaternion.Euler(_zero);


        if (_currentPrefab != null && _transforms.TryGetValue(_currentPrefab, out var t))
        {
            t.parent = null;
            _currentPrefab.SetActive(false);
        }


        if (!_quaternions.TryGetValue(_item.itemPrefab, out var rotation))
        {
            rotation = _item.itemPrefab.transform.rotation;
            _quaternions.Add(_item.itemPrefab, rotation);
        }

        var obj = GameManager.instance.pooler.SpawnFromPool(null, _item.itemPrefab.name, _item.itemPrefab, _zero, rotation);
        
        if (!_outlines.TryGetValue(obj, out var outline))
        {
            outline = obj.GetComponent<Outline>();
            _outlines.Add(obj, outline);
        }

        if (!_transforms.TryGetValue(obj, out var trans))
        {
            trans = obj.transform;
            _transforms.Add(obj, trans);
        }

        trans.SetParent(_thisTransform);
        trans.localPosition = _zero;
        _currentPrefab = obj;
        _outline = outline;
    }

    public void SetupItemDrop(Vector3 initPos, ItemObject item, int amount)
    {
        if (item == null)
        {
            _thisObject.SetActive(false);
            return;
        }

        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }

        _item = item;
        _amount = amount;
        Initialize(initPos);
    }

    void Update()
    {
        var distanceToPlayer = Vector3.Distance(_playerTransform.position, _thisTransform.position);
        if (distanceToPlayer <= _interactableDistance && GameManager.instance.inputHandler.currentMonster < 0)
        {
            _isInteractable = true;

            GameManager.instance.uiManager.itemDropUi.Subscribe(this, _item, _amount);

            if (_outline != null)
            {
                _outline.enabled = true;
                _outline.OutlineMode = Outline.Mode.OutlineVisible;
            }
        }
        else
        {
            if (_outline != null)
            {
                _outline.enabled = false;
            }
            
            _isInteractable = false;
            GameManager.instance.uiManager.itemDropUi.Unsubscribe(this);
        }

        if (!GameManager.instance.inputHandler.interact || !_isInteractable) return;
        GameManager.instance.inputHandler.interact = false;
        _isInteractable = false;

        Interact(_player);
    }

    public void Interact(Player player)
    {
        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }

        StopAllCoroutines();
        StartCoroutine(FollowPlayer(_playerTransform, 1));
    }

    IEnumerator GoToPosition(Vector3 toPos, float seconds)
    {
        var timeElapsed = 0f;
        while (timeElapsed < seconds)
        {
            _thisTransform.position = Vector3.Lerp(_thisTransform.position, toPos, timeElapsed / seconds);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (_thisTransform.position != toPos)
        {
            _thisTransform.position = toPos;
        }
    }

    IEnumerator FollowPlayer(Transform playerTransform, float seconds)
    {
        var timeElapsed = 0f;
        while (timeElapsed < seconds)
        {
            _thisTransform.position = Vector3.Lerp(_thisTransform.position, playerTransform.position, timeElapsed / seconds);
            GameManager.instance.uiManager.itemDropUi.Unsubscribe(this);

            if (Vector3.Distance(_thisTransform.position, playerTransform.position) <= .5f)
            {
                StopAllCoroutines();
                GameManager.instance.uiManager.itemDropUi.Unsubscribe(this);
                _player.playerInventory.AddItemInPlayerInventory(_item, _amount);
                GameManager.instance.audioManager.PlaySFX("Confirmation");
                _thisObject.SetActive(false);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _thisObject.SetActive(false);
    }
}
