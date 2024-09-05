using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateMapData")]
public class MapData : ScriptableObject
{
    [HideInInspector]
    public int GridSize = 1;
    
    [HideInInspector]
    public int Layer;
    
    [HideInInspector]
    public MapData_Grid Grids;

    [HideInInspector]
    public List<MapData_Obstacle> Obstacles;

    [HideInInspector]
    [SerializeField]
    public Dictionary<string, int> _temp;
}
[Serializable]
public class MapData_Grid
{
    public List<int2> _pos;
    public List<int> _value;

    public List<int2> Pos => _pos;
    public List<int> Value => _value;

    public int this[int2 pos]
    {
        get
        {
            return _value[_pos.IndexOf(pos)];
        }

        set
        {
            _value[_pos.IndexOf(pos)] = value;
        }
    }

    public bool TryAdd(int2 key, int value)
    {
        if (_pos.Contains(key)) return false;

        _pos.Add(key);
        _value.Add(value);
        return true;
    }

    public bool Remove(int2 key)
    {
        if (!_pos.Contains(key)) return false;

        var index = _pos.IndexOf(key);
        _pos.RemoveAt(index);
        _value.RemoveAt(index);
        return true;
    }

    public bool ContainsKey(int2 key)
    {
        return _pos.Contains(key);
    }
    
    public void Clear()
    {
        _pos.Clear();
        _value.Clear();
    }
}


[Serializable]
public class MapData_Obstacle
{    
    public string Name;
    public List<Vector3> Vertexs;
}
