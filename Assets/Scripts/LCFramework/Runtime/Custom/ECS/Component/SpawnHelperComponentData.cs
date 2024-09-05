
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public struct SpawnHelperComponentData : IComponentData
{
    public NativeParallelHashMap<long, NativeList<EntityAttributeBuffer>> SpawnCustomAttributeMap;
    public NativeList<long> FreePool;

    private long _spawnDataId;

    [BurstCompile]
    public void ReleaseKey(long key)
    {
        FreePool.Add(key);
    }

    [BurstCompile]
    public long PushSpawnCustomAttribute(EntityAttributeBuffer v1)
    {
        var key = (long)0;
        var len = FreePool.Length;
        if (len >= 1)
        {
            var useKeyIndex = len - 1;
            key = FreePool[useKeyIndex];
            FreePool.RemoveAt(useKeyIndex);
        }
        else
        {
            key = ++_spawnDataId;
        }
        
        SpawnCustomAttributeMap.TryAdd(key, new NativeList<EntityAttributeBuffer>(1 << 4, Allocator.Persistent));
        SpawnCustomAttributeMap[key].Clear();
        SpawnCustomAttributeMap[key].Add(v1);

        return key;
    }

    [BurstCompile]
    public long PushSpawnCustomAttribute(EntityAttributeBuffer v1, EntityAttributeBuffer v2)
    {
        var key = (long)0;
        var len = FreePool.Length;
        if (len >= 1)
        {
            var useKeyIndex = len - 1;
            key = FreePool[useKeyIndex];
            FreePool.RemoveAt(useKeyIndex);
        }
        else
        {
            key = ++_spawnDataId;
        }

        SpawnCustomAttributeMap.TryAdd(key, new NativeList<EntityAttributeBuffer>(1 << 4, Allocator.Persistent));
        SpawnCustomAttributeMap[key].Clear();
        SpawnCustomAttributeMap[key].Add(v1);
        SpawnCustomAttributeMap[key].Add(v2);

        return key;
    }

    [BurstCompile]
    public long PushSpawnCustomAttribute(EntityAttributeBuffer v1, EntityAttributeBuffer v2, EntityAttributeBuffer v3)
    {
        var key = (long)0;
        var len = FreePool.Length;
        if (len >= 1)
        {
            var useKeyIndex = len - 1;
            key = FreePool[useKeyIndex];
            FreePool.RemoveAt(useKeyIndex);
        }
        else
        {
            key = ++_spawnDataId;
        }

        SpawnCustomAttributeMap.TryAdd(key, new NativeList<EntityAttributeBuffer>(1 << 4, Allocator.Persistent));
        SpawnCustomAttributeMap[key].Clear();
        SpawnCustomAttributeMap[key].Add(v1);
        SpawnCustomAttributeMap[key].Add(v2);
        SpawnCustomAttributeMap[key].Add(v3);

        return key;
    }

    [BurstCompile]
    public long PushSpawnCustomAttribute(EntityAttributeBuffer v1, EntityAttributeBuffer v2, EntityAttributeBuffer v3, EntityAttributeBuffer v4)
    {
        var key = (long)0;
        var len = FreePool.Length;
        if (len >= 1)
        {
            var useKeyIndex = len - 1;
            key = FreePool[useKeyIndex];
            FreePool.RemoveAt(useKeyIndex);
        }
        else
        {
            key = ++_spawnDataId;
        }

        SpawnCustomAttributeMap.TryAdd(key, new NativeList<EntityAttributeBuffer>(1 << 4, Allocator.Persistent));
        SpawnCustomAttributeMap[key].Clear();
        SpawnCustomAttributeMap[key].Add(v1);
        SpawnCustomAttributeMap[key].Add(v2);
        SpawnCustomAttributeMap[key].Add(v3);
        SpawnCustomAttributeMap[key].Add(v4);

        return key;
    }
}