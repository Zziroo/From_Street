using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileInfomations
{
    [SerializeField] private GameObject _obj = null;
    [SerializeField] private ETileTypes _type = ETileTypes.Pavement;

    [SerializeField] private float _weight = 0f;

    [SerializeField] private int _minValue = 0;
    [SerializeField] private int _maxValue = 0;
    [SerializeField] private int _objSize = 0;

    public GameObject Prefab { get { return _obj; } }

    public ETileTypes TileType { get { return _type; } }

    public float Weight { get { return _weight; } }

    public int MinValue { get { return _minValue; } }

    public int MaxValue { get { return _maxValue; } }

    public int PoolingObjectSize { get { return _objSize; } }
}

public class RandomTiles : MonoBehaviour
{
    [SerializeField] private List<TileInfomations> _tileInfos = null;

    [SerializeField] private FixedObstacleSpawn _fixedObstacleSpawn = null;

    private Dictionary<ETileTypes, ObjectPool> _tileDictionaries = new Dictionary<ETileTypes, ObjectPool>();

    private Queue<GameObject> _createdTiles = new Queue<GameObject>();

    private Queue<ETileTypes> _listTileType = new Queue<ETileTypes>();

    private FixedObstaclePositioningMap _fixedObstaclePositioningMap = new FixedObstaclePositioningMap();

    private Vector3 _currPos = new Vector3(0f, 0f, 6f);

    private ETileTypes _lastTileType = ETileTypes.Pavement;

    private const int READY_TILE_NUMBER = 2;
    private const int MAX_TILE_NUMBER = 20;

    private void Start()
    {
        for (int i = 0; i < _tileInfos.Count; ++i)
        {
            ObjectPool tempPool = new ObjectPool();

            _tileDictionaries[_tileInfos[i].TileType] = tempPool;

            tempPool.InitializeObjectPool(_tileInfos[i].PoolingObjectSize, _tileInfos[i].Prefab);
        }

        CreateInitTiles();
    }

    public void ReturnTile(ETileTypes type, GameObject obj)
    {
        _createdTiles.Dequeue();

        _tileDictionaries[type].ReturnObject(obj);

        if (ConstantValue.EMPTY == _listTileType.Count)
        {
            ListUpTileType();
        }

        PushTile(_listTileType.Dequeue());
    }

    private void CreateInitTiles()
    {
        for (int i = 0; i < READY_TILE_NUMBER; ++i)
        {
            PushTile(ETileTypes.Pavement);
        }

        while (_createdTiles.Count <= MAX_TILE_NUMBER)
        {
            ListUpTileType();

            for (int i = 0; i < _listTileType.Count;)
            {
                PushTile(_listTileType.Dequeue());
            }
        }
    }

    private void ListUpTileType()
    {
        ETileTypes type = SelectTileType();

        while (type == _lastTileType)
        {
            type = SelectTileType();
        }

        _lastTileType = type;

        int _randomTileNumber = UnityEngine.Random.Range(_tileInfos[(int)type].MinValue, _tileInfos[(int)type].MaxValue);

        for (int i = 0; i < _randomTileNumber; ++i)
        {
            _listTileType.Enqueue(type);
        }
    }

    private void PushTile(ETileTypes type)
    {
        GameObject obj = _tileDictionaries[type].GiveObject();

        _fixedObstaclePositioningMap.GetTileType(type);

        obj.transform.position = _currPos;

        _fixedObstacleSpawn.SetFixedObstacle(_fixedObstaclePositioningMap.CreatablePosition);

        _createdTiles.Enqueue(obj);

        _currPos += Vector3.forward * ConstantValue.TILE_SIZE;
    }

    private ETileTypes SelectTileType()
    {
        float total = 0f;

        for (int i = 0; i < _tileInfos.Count; ++i)
        {
            total += _tileInfos[i].Weight;
        }

        float randomValue = UnityEngine.Random.value * total;

        for (int i = 0; i < _tileInfos.Count; ++i)
        {
            if (randomValue < _tileInfos[i].Weight)
            {
                return (ETileTypes)i;
            }
            else
            {
                randomValue -= _tileInfos[i].Weight;
            }
        }

        return ETileTypes.Pavement;
    }
}