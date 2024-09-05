
using cfg;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EntitySearchHelper : IComponentData
{
    [BurstCompile]
    public struct SearchData : IEquatable<SearchData>
    {
        public Entity entity;
        public LocalTransform tsData;
        public EntityTeam Team;
        public float Radius;

        [BurstCompile]
        public bool Equals(SearchData other)
        {
            return entity == other.entity;
        }
        public static bool operator ==(SearchData lhs, SearchData rhs)
        {
            return lhs.entity.Index == rhs.entity.Index && lhs.entity.Version == rhs.entity.Version;
        }

        public static bool operator !=(SearchData lhs, SearchData rhs)
        {
            return !(lhs == rhs);
        }

        public static SearchData Null => new SearchData() { entity = Entity.Null };
    }

    public NativeParallelMultiHashMap<int3, SearchData> EntitySearchMap;
    public NativeParallelHashSet<int3> Keys;

    //[BurstCompile]
    //public bool QueryEntity(EntityTeam team, float3 pos, float radius, out NativeList<SearchData> findEntities)
    //{
    //    findEntities = new NativeList<SearchData>(Allocator.Temp);

    //    var gridPosMin = (pos + new float3(-1, 0, -1) * radius);
    //    var gridPosMax = (pos + new float3(1, 0, 1) * radius);

    //    var keyMin = new int2((int)math.round(gridPosMin.x), (int)math.round(gridPosMin.z));
    //    var keyMax = new int2((int)math.round(gridPosMax.x), (int)math.round(gridPosMax.z));

    //    var keys = new NativeList<int2>(Allocator.Temp);
    //    for (int x = keyMin.x; x <= keyMax.x; x++)
    //    {
    //        for (int z = keyMin.y; z <= keyMax.y; z++)
    //        {
    //            var key = new int2(x, z);
    //            if (!keys.Contains(key)) keys.Add(key);
    //        }
    //    }

    //    for (int i = 0; i < keys.Length; i++)
    //    {
    //        var key = keys[i];
    //        if(EntitySearchMap.TryGetFirstValue(key, out var searchData, out var token))
    //        {
    //            do
    //            {
    //                if (searchData.Team != team) continue;
    //                if (findEntities.Contains(searchData)) continue;
    //                if (math.distance(pos, searchData.tsData.Position) > radius) continue;
    //                findEntities.Add(searchData);
    //            } while (EntitySearchMap.TryGetNextValue(out searchData, ref token));
    //        }
    //    }

    //    keys.Dispose();

    //    return findEntities.Length > 0;
    //}

    //[BurstCompile]
    //public bool QueryEntityExceptTeam(EntityTeam team, float3 pos, float radius, out NativeList<SearchData> findEntities)
    //{
    //    findEntities = new NativeList<SearchData>(Allocator.Temp);

    //    var gridPosMin = (pos + new float3(-1, 0, -1) * radius);
    //    var gridPosMax = (pos + new float3(1, 0, 1) * radius);

    //    var keyMin = new int2((int)math.round(gridPosMin.x), (int)math.round(gridPosMin.z));
    //    var keyMax = new int2((int)math.round(gridPosMax.x), (int)math.round(gridPosMax.z));

    //    var keys = new NativeList<int2>(Allocator.Temp);
    //    for (int x = keyMin.x; x <= keyMax.x; x++)
    //    {
    //        for (int z = keyMin.y; z <= keyMax.y; z++)
    //        {
    //            var key = new int2(x, z);
    //            if (!keys.Contains(key)) keys.Add(key);
    //        }
    //    }

    //    for (int i = 0; i < keys.Length; i++)
    //    {
    //        var key = keys[i];
    //        if (EntitySearchMap.TryGetFirstValue(key, out var searchData, out var token))
    //        {
    //            do
    //            {
    //                if (searchData.Team == team) continue;
    //                if (findEntities.Contains(searchData)) continue;
    //                if (math.distance(pos, searchData.tsData.Position) > radius) continue;
    //                findEntities.Add(searchData);
    //            } while (EntitySearchMap.TryGetNextValue(out searchData, ref token));
    //        }
    //    }

    //    keys.Dispose();

    //    return findEntities.Length > 0;
    //}

    [BurstCompile]
    public bool QueryEntityNearest(EntityTeam team, float3 pos, out SearchData nearest)
    {
        nearest = SearchData.Null;
        var keys = Keys.ToNativeArray(Allocator.Temp);
        var gridKey = new int2((int)math.round(pos.x / EntityGridSystem.Size), (int)math.round(pos.z / EntityGridSystem.Size));
        var intTeam = (int)team;

        var nearestGridKey = new int3(9999, 9999, 9999);
        var nearestDistance = 0f;

        for (int i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            if (key.z != intTeam) continue;

            var tempDistance = math.abs(gridKey.x - key.x) + math.abs(gridKey.y - key.y);
            if (tempDistance < nearestDistance || nearestGridKey.Equals(new int3(9999, 9999, 9999)))
            {
                nearestGridKey = key;
                nearestDistance = tempDistance;
                continue;
            }
        }

        if (EntitySearchMap.TryGetFirstValue(nearestGridKey, out var searchData, out var token))
        {
            do
            {
                if (searchData.Team == team) continue;

                if (nearest == SearchData.Null)
                {
                    nearest = searchData;
                    continue;
                }

                if (math.distance(nearest.tsData.Position, pos) > math.distance(searchData.tsData.Position, pos))
                {
                    nearest = searchData;
                    continue;
                }

            } while (EntitySearchMap.TryGetNextValue(out searchData, ref token));
        }

        keys.Dispose();

        return nearest != SearchData.Null;
    }

    [BurstCompile]
    public bool QueryEntityNearestExceptTeam(EntityTeam team, float3 pos, out SearchData nearest)
    {
        nearest = SearchData.Null;
        var keys = Keys.ToNativeArray(Allocator.Temp);
        var gridKey = new int2((int)math.round(pos.x / EntityGridSystem.Size), (int)math.round(pos.z / EntityGridSystem.Size));
        var intTeam = (int)team;

        var nearestGridKey = new int3(9999, 9999, 9999);
        var nearestDistance = 0f;

        for (int i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            if (key.z == intTeam) continue;

            var tempDistance = math.abs(gridKey.x - key.x) + math.abs(gridKey.y - key.y);
            if (tempDistance < nearestDistance || nearestGridKey.Equals(new int3(9999, 9999, 9999)))
            {
                nearestGridKey = key;
                nearestDistance = tempDistance;
                continue;
            }
        }

        if (EntitySearchMap.TryGetFirstValue(nearestGridKey, out var searchData, out var token))
        {
            do
            {
                if (searchData.Team == team) continue;

                if (nearest == SearchData.Null)
                {
                    nearest = searchData;
                    continue;
                }

                if (math.distance(nearest.tsData.Position, pos) > math.distance(searchData.tsData.Position, pos))
                {
                    nearest = searchData;
                    continue;
                }

            } while (EntitySearchMap.TryGetNextValue(out searchData, ref token));
        }

        keys.Dispose();

        return nearest != SearchData.Null;
    }
}