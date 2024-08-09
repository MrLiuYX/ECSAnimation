using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Native.Audio
{
    public interface IAudioGroup
    {
        public int GroupID { get; }
        public bool HasAudio(int id);
        public void PlayAudio(int id, AudioData data, AudioClip clip);
        public void StopAudio(int id);
    }  
}
