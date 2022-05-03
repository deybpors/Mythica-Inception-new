using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using MyBox;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool activated;
    public GameObject itemDropPrefab;
    public List<ItemObject> items;
    public float xAxis;
    public float zAxis;
    public float randomSpawnTimeMax;

    public int noOfItemsLimit;
    [ReadOnly] public int currentNoOfItems;

    private Coroutine _waitSpawn;
    private Vector3 _spawnerPosition;
    private Transform _thisTransform;
    private int _itemsCount;

    private Dictionary<GameObject, ItemDrop> _itemDrops = new Dictionary<GameObject, ItemDrop>();
    private HashSet<GameObject> _currentItems = new HashSet<GameObject>();

    void Start()
    {
        _thisTransform = transform;
        _spawnerPosition = _thisTransform.position;
        _itemsCount = items.Count;

        if(!activated) return;
        InvokeRepeating(nameof(SpawnItems), 0, 1);
    }

    private void SpawnItems()
    {
        if(!activated) return;
        if (currentNoOfItems >= noOfItemsLimit) return;
        if (GameManager.instance == null) return;
        if (GameManager.instance.player == null) return;
        var distance = Vector3.Distance(_thisTransform.position, GameManager.instance.player.playerTransform.position);
        if (distance > 30) return;

        if (_waitSpawn != null) return;

        Spawn();
        _waitSpawn = StartCoroutine(WaitSpawnTime());
    }

    void Spawn()
    {
        var currentZ = Random.Range(_spawnerPosition.z - zAxis, _spawnerPosition.z + zAxis);
        var currentX = Random.Range(_spawnerPosition.x - xAxis, _spawnerPosition.x + xAxis);
        var newPosition = new Vector3(currentX, _spawnerPosition.y, currentZ);
        var obj = GameManager.instance.pooler.SpawnFromPool(null, itemDropPrefab.name, itemDropPrefab, newPosition, Quaternion.identity);

        if (!_itemDrops.TryGetValue(obj, out var drop))
        {
            drop = obj.GetComponent<ItemDrop>();
            _itemDrops.Add(obj, drop);
        }

        drop.SetupItemDrop(_spawnerPosition, items[Random.Range(0, _itemsCount)], 1, this);
        if (_currentItems.Add(obj))
        {
            currentNoOfItems++;
        }
    }

    public void RemoveItemInCurrentItems(GameObject itemObject)
    {
        if (_currentItems.Remove(itemObject))
        {
            currentNoOfItems--;
        }
    }

    IEnumerator WaitSpawnTime()
    {
        yield return new WaitForSeconds(Random.Range(1, randomSpawnTimeMax + 1));
        _waitSpawn = null;
    }
}
