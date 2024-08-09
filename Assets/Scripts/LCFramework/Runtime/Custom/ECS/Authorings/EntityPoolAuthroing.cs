using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityPoolAuthroing : MonoBehaviour
{
    [Serializable]
    public struct EntityPoolAuthoringData
    {
        [SerializeField]
        public int PrefabId;
        [SerializeField]
        public GameObject Prefab;
    }

    [SerializeField]
    private List<EntityPoolAuthoringData> _entityPoolAuthoringDatas;

    public class EntityPoolAuthroingBaker : Baker<EntityPoolAuthroing>
    {
        public override void Bake(EntityPoolAuthroing authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var buffer =  AddBuffer<EntityPrefabBuffer>(entity);
            var assetDictComponentData = new AnimationRendererAssetDictComponentData()
            {
                AssetPathDict = new Dictionary<(string,string), int>(),
                AssetDict = new Dictionary<int, AnimationRendererAssetDictComponentData.DictData>(),
            };

            for (int i = 0; i < authoring._entityPoolAuthoringDatas.Count; i++)
            {
                var data = authoring._entityPoolAuthoringDatas[i];
                buffer.Add(new EntityPrefabBuffer()
                {
                    PrefabId = data.PrefabId,
                    Prefab = GetEntity(data.Prefab, TransformUsageFlags.Dynamic)
                });

                var animation = data.Prefab.GetComponent<ECSAnimationAuthoring>();

                if(animation)
                {
                    var key = (animation.MeshPath, animation.MatPath);
                    if (!assetDictComponentData.AssetPathDict.TryGetValue(key, out var index))
                    {
                        index = ++assetDictComponentData.DictCount;
                        assetDictComponentData.AssetDict.Add(index, new AnimationRendererAssetDictComponentData.DictData()
                        {
                            Mesh = null,
                            Mat = null,

                            MeshPath = animation.MeshPath,
                            MatPath = animation.MatPath,
                            State = AnimationRendererAssetDictComponentData.AssetState.None,
                        });
                        assetDictComponentData.AssetPathDict.Add(key, index);
                    }
                }
            }

            AddComponentObject(entity, assetDictComponentData);
            AddComponent<EntityPoolTag>(entity, new EntityPoolTag()
            {
                 Owner = entity,
            });
            AddBuffer<EntitySpawnBuffer>(entity);
            AddBuffer<EntityUnSpawnBuffer>(entity);
        }
    }
}
