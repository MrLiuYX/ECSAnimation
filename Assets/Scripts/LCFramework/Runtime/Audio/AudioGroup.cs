using Native.Component;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Native.Audio
{
    public class AudioGroup : IAudioGroup, IReference
    {
        private class InternalData : IReference
        {
            public AudioData AudioData;
            public AudioSource AudioSource;
            public AudioClip Clip;
            public int ID;
            public float Time;            

            public static InternalData Create(int id, AudioData audioData, AudioSource audioSource, AudioClip clip)
            {
                var data = ReferencePool.Acquire<InternalData>();
                data.AudioSource = audioSource;
                data.AudioData = audioData;
                data.ID = id;
                data.Time = UnityEngine.Time.time;
                data.Clip = clip;
                return data;
            }

            public void RefreshNewAudio(int id, AudioData audioData, AudioClip clip)
            {
                AudioData = audioData;
                ID = id;
                Time = UnityEngine.Time.time;
                Clip = clip;
                AudioSource.transform.name = $"Audio:{id}";
                ReferencePool.Release(AudioData);                
            }

            public bool CheckPlaying()
            {
                return AudioSource.isPlaying;
            }

            public void PlayAudio()
            {
                AudioSource.clip = Clip;
                AudioSource.loop = AudioData.Loop;
                AudioSource.volume = AudioData.Volume;
                AudioSource.spatialBlend = AudioData.Audio2D ? 0 : 1;
                if(!AudioData.Audio2D)
                {
                    AudioSource.rolloffMode = AudioData.RolloffMode;
                    AudioSource.minDistance = AudioData.MinDistance;
                    AudioSource.maxDistance = AudioData.MaxDistance;
                    AudioSource.spread = 360;
                    if(AudioSource.rolloffMode == AudioRolloffMode.Custom)
                    {
                        AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AudioData.Curve);
                    }
                }
                AudioSource.Play();
            }

            public void Stop()
            {
                AudioSource.Stop();
            }

            public void Clear()
            {
                ReferencePool.Release(AudioData);
            }
        }

        public static AudioGroup Create(int groupId, Transform ts)
        {
            var data = ReferencePool.Acquire<AudioGroup>();
            data._groupId = groupId;
            data._ts = ts;
            data._audioPlayDatas = data._audioPlayDatas ?? new List<InternalData>();
            data._hasAudioId = data._hasAudioId ?? new List<int>();
            if (data._audioPlayDatas.Count != 0) data._audioPlayDatas.Clear();
            if (data._hasAudioId.Count != 0) data._hasAudioId.Clear();
            return data;
        }

        public void Clear()
        {
            
        }

        private int _groupId;
        private Transform _ts;
        private List<InternalData> _audioPlayDatas;
        private List<int> _hasAudioId;        

        public int GroupID { get { return _groupId; } }

        public bool HasAudio(int id)
        {
            return _hasAudioId.Contains(id);
        }

        public void PlayAudio(int id, AudioData audioData, AudioClip clip)
        {
            for (int i = 0; i < _audioPlayDatas.Count; i++)
            {
                var data = _audioPlayDatas[i];
                if (!data.CheckPlaying())
                {
                    _hasAudioId.Remove(data.ID);
                    _hasAudioId.Add(id);
                    data.RefreshNewAudio(id, audioData, clip);
                    data.PlayAudio();
                    return;
                }
            }

            if(_audioPlayDatas.Count != LaunchComponent.Audio.MaxAudioChanel)
            {
                InternalPlayAudio(id, audioData, clip);
            }
            else
            {
                InternalRecycleAndPlayAudio(id, audioData, clip);
            }
        }

        public void StopAudio(int id)
        {
            for (int i = 0; i < _audioPlayDatas.Count; i++)
            {
                if(_audioPlayDatas[i].ID == id)
                {
                    _audioPlayDatas[i].Stop();
                    break;
                }
            }
        }

        private void InternalPlayAudio(int id, AudioData audioData, AudioClip clip)
        {
            var ts = new GameObject($"Audio:{id}").transform;
            ts.SetParent(_ts);
            ts.position = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localScale = Vector3.zero;
            var audioSource = ts.gameObject.AddComponent<AudioSource>();
            var data = InternalData.Create(id, audioData, audioSource, clip);
            _audioPlayDatas.Add(data);
            data.PlayAudio();
            _hasAudioId.Add(id);
        }

        private void InternalRecycleAndPlayAudio(int id, AudioData audioData, AudioClip clip)
        {
            _audioPlayDatas.Sort((x, y) =>
            {
                if (y.Time < x.Time) return 1;
                return -1;
            });
            _hasAudioId.Remove(_audioPlayDatas[0].ID);
            _hasAudioId.Add(id);
            _audioPlayDatas[0].RefreshNewAudio(id, audioData, clip);
            _audioPlayDatas[0].PlayAudio();
        }
    }
}
