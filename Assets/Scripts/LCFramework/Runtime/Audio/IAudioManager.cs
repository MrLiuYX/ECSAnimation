using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Native.Audio
{
    public class AudioData : IReference
    {
        public bool Loop;
        public float Volume;
        public float MinDistance;
        public float MaxDistance;
        public AudioRolloffMode RolloffMode;
        public AnimationCurve Curve;
        public bool Audio2D;

        public static AudioData Create2D(
            bool loop
            , float volume)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = true;
            return data;
        }

        public static AudioData Create3D(
            bool loop
            , float volume)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = false;
            data.RolloffMode = AudioRolloffMode.Linear;
            data.MinDistance = 0;
            data.MaxDistance = 500;
            return data;
        }

        public static AudioData Create3D(
            bool loop
            , float volume
            , AnimationCurve curve)
        {
            var data = ReferencePool.Acquire<AudioData>();
            data.Loop = loop;
            data.Volume = volume;
            data.Audio2D = false;
            data.RolloffMode = AudioRolloffMode.Custom;
            data.MinDistance = 0;
            data.MaxDistance = 500;
            data.Curve = curve;
            return data;
        }

        public void Clear()
        {
            Curve = null;
        }
    }

    public interface IAudioManager
    {
        /// <summary>
        /// 音效组件挂载物体
        /// </summary>
        public Transform ComponentTs { get; }
        /// <summary>
        /// 每个组最大音效播放数量
        /// </summary>
        public int MaxAudioChanel { get; }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path">音调地址</param>
        /// <param name="group">音效所属组</param>
        /// <param name="data">播放音效的数据</param>
        /// <returns></returns>
        public int PlayAudio(string path, int group, AudioData data);

        /// <summary>
        /// 停止音效播放
        /// </summary>
        /// <param name="audioId">音效ID</param>
        public void StopAudio(int audioId);
    }
}
