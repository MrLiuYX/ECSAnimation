using Native.Audio;
using System;
using UnityEngine;

namespace Native.Component
{
    public class AudioComponent : ComponentBase
    {
        [SerializeField]
        private int _maxAudioChanel;

        public override void ManagerSet()
        {
            Manager = (Native.Construct.IManager)LaunchComponent.Audio;
            ((AudioManager)Manager).MaxAudioChanel = _maxAudioChanel;
            ((AudioManager)Manager).ComponentTs = gameObject.transform;
        }
    }
}
