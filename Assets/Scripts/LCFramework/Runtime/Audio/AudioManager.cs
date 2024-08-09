using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Native.Construct;
using System.IO;
using Native.Resource;
using Native.Component;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Native.Audio
{
    public class AudioManager : ManagerBase<IAudioManager>, IAudioManager
    {
        private class InternalData : IReference
        {
            public int ID;
            public int GroupId;
            public AudioData AudioData;

            public static InternalData Create(int id, int groupId, AudioData audioData)
            {
                var data = ReferencePool.Acquire<InternalData>();
                data.ID = id;
                data.AudioData = audioData;
                data.GroupId = groupId;
                return data;
            }

            public void Clear()
            {
                AudioData = null;
            }
        }

        public const string AudioPath = "Assets/AssetBundleRes/Main/Audios/{0}/{1}.mp3";
        public int MaxAudioChanel { get; set; }

        public Transform ComponentTs { get; set; }

        private Dictionary<int, IAudioGroup> _audioGroups;
        private int _id;

        private LoadAssetCallback _loadCallback;

        public override void Init()
        {
            base.Init();
            _id = -1;
            _audioGroups = new Dictionary<int, IAudioGroup>();
            _loadCallback = new LoadAssetCallback(LoadClipSuccessCallback, LoadClipFailCallback);
        }

        public int PlayAudio(string path, int group, AudioData data)
        {

            _id++;

            if (!_audioGroups.ContainsKey(group))
            {
                var ts =  new GameObject($"AudioGroup:{group}").transform;
                ts.SetParent(ComponentTs);
                ts.position = Vector3.zero;
                ts.rotation = Quaternion.identity;
                ts.localScale = Vector3.one;
                _audioGroups.Add(group, AudioGroup.Create(group, ts));
            }
            
            LaunchComponent.Resource.LoadAsset(path, _loadCallback, InternalData.Create(_id, group, data));

            return _id;
        }

        public void StopAudio(int audioId)
        {
            foreach (var item in _audioGroups)
            {                
                if(item.Value.HasAudio(audioId))
                {
                    item.Value.StopAudio(audioId);
                    break;
                }
            }
        }

        public override void SubInspectorDraw()
        {
            base.SubInspectorDraw();

        }

        private void LoadClipSuccessCallback(string path, object asset, float time, object userData)
        {
            var data = (InternalData)userData;
            _audioGroups[data.GroupId].PlayAudio(data.ID, data.AudioData, (AudioClip)asset);
        }

        private void LoadClipFailCallback(string path, float time, object userData)
        {
            var data = (InternalData)userData;
            throw new System.Exception($"º”‘ÿ{path} ß∞‹");
        }
    }
}


