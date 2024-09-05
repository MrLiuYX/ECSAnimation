using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(EntityLogicGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial class EntityCommonValueSystem : SystemBase
{
    private Camera _camera;
    private float _cameraSize;

    protected override void OnCreate()
    {
        EntityManager.AddComponent<EntityCommonValueComponentData>(SystemHandle);
        var data = SystemAPI.GetSingletonRW<EntityCommonValueComponentData>();
        data.ValueRW.ColorMap = new Unity.Collections.NativeHashMap<int, Unity.Mathematics.float4>(1 << 6, Allocator.Persistent);

        data.ValueRW.ColorMap.Add(1, new Unity.Mathematics.float4(0, 0, 1, 1));
        data.ValueRW.ColorMap.Add(2, new Unity.Mathematics.float4(1, 0, 0, 1));
    }

    protected override void OnUpdate()
    {
        if(Camera.main != null && _camera == null)
        {
            _camera = Camera.main;
            _cameraSize = _camera.orthographicSize;
        }

        var commonValue =  SystemAPI.GetSingletonRW<EntityCommonValueComponentData>();
        var min = Camera.main.ScreenPointToRay(Vector2.zero);
        var max = Camera.main.ScreenPointToRay(new Vector2(Screen.width, Screen.height));

        var minPos = min.origin + min.origin.y / (min.origin.y - (min.origin + min.direction).y) * min.direction;
        minPos.y = 0;
        var maxPos = max.origin + max.origin.y / (max.origin.y - (max.origin + max.direction).y) * max.direction;
        maxPos.y = 0;

        commonValue.ValueRW.ScreenMin = new Unity.Mathematics.float2(minPos.x, minPos.z);
        commonValue.ValueRW.ScreenMax = new Unity.Mathematics.float2(maxPos.x, maxPos.z);
        commonValue.ValueRW.CheckScreenMin = commonValue.ValueRO.ScreenMin - new float2(1,1);
        commonValue.ValueRW.CheckScreenMax = commonValue.ValueRO.ScreenMax + new float2(1,1);
        commonValue.ValueRW.CameraDistanceRatio = _cameraSize < _camera.orthographicSize
            ? _cameraSize / Camera.main.orthographicSize
            : math.clamp((Camera.main.orthographicSize / _cameraSize), 0.5f, 1);
    }
}