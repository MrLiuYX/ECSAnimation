
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(EntityPoolGroup))]
[UpdateBefore(typeof(EntitySpawnSystem))]
public partial class PlayerInputSystem : SystemBase
{
    private int index = 0;
    private Dictionary<int, int> _yOffset;
    public int Count;

    protected override void OnCreate()
    {
        _yOffset = new Dictionary<int, int>();  
    }

    protected override void OnUpdate()
    {
        var buffer = SystemAPI.GetSingletonBuffer<EntityPrefabBuffer>();
        if (Input.GetMouseButtonDown(1))
        {
            index = ++index % buffer.Length;
            Debug.LogError($"当前Debug产生的实体Id为:{buffer[index].PrefabId}");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Clear();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Create(index, 10000);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo, 10000))
            {
                var spawnBuffer =  SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
                var tsData = LocalTransform.Identity;
                tsData.Position = hitInfo.point;
                spawnBuffer.Add(new EntitySpawnBuffer()
                {
                    EntityId = 10001 + index,
                    PrefabId = 10001 + index,
                    TsData = tsData,
                });
                Count++;
            }
        }
    }

    public void Create(int index, int count)
    {
        if (!Enabled) return;

        var spawnBuffer = SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        var offset = index % 4;
        if (!_yOffset.ContainsKey(offset))
        {
            _yOffset.Add(offset, -1);
        }
        _yOffset[offset]++;
        var countSq = Mathf.Sqrt(count);
        for (int i = 0; i < count; i++)
        {
            var tsData = LocalTransform.Identity;
            tsData.Position = new float3((i % countSq) * ((offset == 0 || offset == 3) ? -1 : 1), _yOffset[offset], i / countSq * ((offset == 0 || offset == 1)? 1 : -1));
            spawnBuffer.Add(new EntitySpawnBuffer()
            {
                EntityId = 10001 + index,
                PrefabId = 10001 + index,
                TsData = tsData,
            });
            Count++;
        }
    }

    public void Clear()
    {
        var unSpawnBuffer = SystemAPI.GetSingletonBuffer<EntityUnSpawnBuffer>();
        var SpawnBuffer = SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        SpawnBuffer.Clear();
        foreach (var instanceTagData in SystemAPI.Query<InstanceTag>())
        {
            unSpawnBuffer.Add(new EntityUnSpawnBuffer()
            {
                PrefabId = instanceTagData.PrefabId,
                Entity = instanceTagData.Self,
            });
        }
        Count = 0;
        _yOffset.Clear();
    }
}
