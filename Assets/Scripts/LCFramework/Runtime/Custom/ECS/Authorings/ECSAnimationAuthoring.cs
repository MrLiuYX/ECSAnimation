
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ECSAnimationAuthoring : MonoBehaviour
{
    [SerializeField]
    public List<AnimationConfigBuffer> animationConfigs;
    [SerializeField]
    public List<AnimationEventConfigBuffer> eventConfigs;
    [SerializeField]
    public AnimationConfigComponentData Config;

    public string MeshPath;
    public string MatPath;

    public class ECSAnimationAuthoringBaker : Baker<ECSAnimationAuthoring>
    {
        public override void Bake(ECSAnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var animBuffer = AddBuffer<AnimationConfigBuffer>(entity);
            var eventBuffer = AddBuffer<AnimationEventConfigBuffer>(entity);
            // 事件
            AddBuffer<EntityAnimationTriggerEventBuffer>(entity);
            AddBuffer<EntityAnimationWaitTriggerEventBuffer>(entity);

            for (int i = 0; i < authoring.animationConfigs.Count; i++)
            {
                animBuffer.Add(authoring.animationConfigs[i]);
            }
            for (int i = 0; i < authoring.eventConfigs.Count; i++)
            {
                eventBuffer.Add(authoring.eventConfigs[i]);
            }

            AddComponent<AnimationConfigComponentData>(entity, authoring.Config);
            AddComponent<AnimationInstanceComponentData>(entity);
            AddComponent<AnimationRuntimeComponentData>(entity, new AnimationRuntimeComponentData()
            {
                Speed = 1,
            });
            AddComponentObject<AnimationRendererPathComponentData>(entity, new AnimationRendererPathComponentData()
            {
                MeshPath = authoring.MeshPath,
                MatPath = authoring.MatPath,
            });
        }
    }
}
