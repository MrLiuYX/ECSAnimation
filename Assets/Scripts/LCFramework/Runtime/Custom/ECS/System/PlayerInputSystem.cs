
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;
using Nebukam.ORCA;
using Random = Unity.Mathematics.Random;
using cfg;

[UpdateInGroup(typeof(EntityPoolGroup))]
[UpdateBefore(typeof(EntitySpawnSystem))]
public partial class PlayerInputSystem : SystemBase
{
    //public partial struct PlayerInputSystemJob:IJobEntity
    //{
    //    [ReadOnly] public float3 Point;
    //    public void Execute(
    //            ref EntityRVOAgentComponentData rvoData
    //            , ref EntityStateComponentData stateData
    //            , Entity entity
    //        )
    //    {
    //        rvoData.TargetPos = Point;
    //        stateData.SearchData = new EntitySearchHelper.SearchData()
    //        {
    //            Team = EntityTeam.Team1,
    //            entity = Entity.Null,
    //            tsData = new LocalTransform()
    //            {
    //                Position = rvoData.TargetPos,
    //            }
    //        };
    //    }
    //}

    private Dictionary<int, int> _yOffset;
    public int Count;
    public Random _random;

    protected override void OnCreate()
    {
        _yOffset = new Dictionary<int, int>();
        _random = new Random(123456);
    }

    protected override void OnUpdate()
    {
        //var buffer = SystemAPI.GetSingletonBuffer<EntityPrefabBuffer>();
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    index = ++index % buffer.Length;
        //    Debug.LogError($"当前Debug产生的实体Id为:{buffer[index].PrefabId}");
        //}

//#if UNITY_EDITOR
//        if (Input.GetMouseButtonDown(1))
//        {
//            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInof))
//            {
//                var hitPoint = hitInof.point;
//                hitPoint.y = 0;
//                Dependency = new PlayerInputSystemJob
//                {
//                    Point = hitPoint,
//                }.ScheduleParallel(Dependency);
//                Dependency.Complete();
//            }
//        }
//#else
//        if (Input.GetMouseButtonDown(0))
//        {
//            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInof))
//            {
//                var hitPoint = hitInof.point;
//                hitPoint.y = 0;
//                Dependency = new PlayerInputSystemJob
//                {
//                    Point = hitPoint,
//                }.ScheduleParallel(Dependency);
//                Dependency.Complete();
//            }
//        }
//#endif


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Clear();
        //}

        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    Create(index, 10000);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if(Physics.Raycast(ray, out var hitInfo, 10000))
        //    {
        //        var spawnBuffer =  SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        //        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        //        var tsData = LocalTransform.Identity;
        //        var point = hitInfo.point;
        //        point.y = 0;
        //        tsData.Position = point + new Vector3(_random.NextFloat(-0.25f, 0.25f), 0, _random.NextFloat(-0.25f, 0.25f));
        //        spawnBuffer.Add(new EntitySpawnBuffer()
        //        {
        //            EntityId = 10001 + index,
        //            PrefabId = 10001 + index,
        //            TsData = tsData,
        //            SpawnId = spawnHelper.ValueRW.PushSpawnCustomAttribute(new EntityAttributeBuffer()
        //            {
        //                AttributeType = cfg.AttributeType.Team,
        //                Value = EntityTeam.Team1.ToInt(),
        //            })
        //        });
        //        Count++;
        //    }
        //}

        //if (Input.GetMouseButtonDown(2))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out var hitInfo, 10000))
        //    {
        //        var spawnBuffer = SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        //        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        //        var tsData = LocalTransform.Identity;
        //        var point = hitInfo.point;
        //        point.y = 0;
        //        tsData.Position = point + new Vector3(_random.NextFloat(-0.25f, 0.25f), 0, _random.NextFloat(-0.25f, 0.25f));
        //        spawnBuffer.Add(new EntitySpawnBuffer()
        //        {
        //            EntityId = 10001 + index,
        //            PrefabId = 10001 + index,
        //            TsData = tsData,
        //            SpawnId = spawnHelper.ValueRW.PushSpawnCustomAttribute(new EntityAttributeBuffer()
        //            {
        //                AttributeType = cfg.AttributeType.Team,
        //                Value = EntityTeam.Team2.ToInt(),
        //            })
        //        });
        //        Count++;
        //    }
        //}

        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");
        var mouseScrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");

        var camera = Camera.main;
        camera.transform.position += new Vector3(hor, 0, ver) * SystemAPI.Time.DeltaTime * 10 * 5;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + mouseScrollWheel * -4, 5, 25);
    }

    public void Create(int index, int count)
    {
        if (!Enabled) return;

        var spawnBuffer = SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        var offset = index % 4;
        if (!_yOffset.ContainsKey(offset))
        {
            _yOffset.Add(offset, -1);
        }
        var countSq = Mathf.Sqrt(count);
        for (int i = 0; i < count; i++)
        {
            var tsData = LocalTransform.Identity;
            tsData.Position = new float3((i % countSq) * ((offset == 0 || offset == 3) ? -1 : 1) + _random.NextFloat(-0.25f, 0.25f), 0, i / countSq * ((offset == 0 || offset == 1) ? 1 : -1) + _random.NextFloat(-0.25f, 0.25f));
            spawnBuffer.Add(new EntitySpawnBuffer()
            {
                EntityId = 10001,
                PrefabId = 10001,
                TsData = tsData,
                SpawnId = spawnHelper.ValueRW.PushSpawnCustomAttribute(new EntityAttributeBuffer()
                {
                    AttributeType = cfg.AttributeType.Team,
                    Value = index + 1,
                })
            });
            Count++;
        }
    }
}
