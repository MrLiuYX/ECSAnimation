
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ECSAnimationAuthoring : MonoBehaviour
{
    [SerializeField]
    public List<EntityAnimationConfigBuffer> animationConfigs;
    [SerializeField]
    public List<EntityAnimationEventConfigBuffer> eventConfigs;
    [SerializeField]
    public EntityAnimationConfigComponentData Config;

    public string MeshPath;
    public string MatPath;

    public class ECSAnimationAuthoringBaker : Baker<ECSAnimationAuthoring>
    {
        public override void Bake(ECSAnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var animBuffer = AddBuffer<EntityAnimationConfigBuffer>(entity);
            var eventBuffer = AddBuffer<EntityAnimationEventConfigBuffer>(entity);
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

            AddComponent<EntityAnimationConfigComponentData>(entity, authoring.Config);
            AddComponent<EntityAnimationInstanceComponentData>(entity);

            AddComponent<EntityAnimationRuntimeComponentData>(entity, new EntityAnimationRuntimeComponentData()
            {
                Speed = 1,
                Duration = 0.2f,
            });

            AddComponentObject<EntityAnimationRendererPathComponentData>(entity, new EntityAnimationRendererPathComponentData()
            {
                MeshPath = authoring.MeshPath,
                MatPath = authoring.MatPath,
            });
        }
    }
}
