using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSAnimationMaker : MonoBehaviour
{
    public struct TexData
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    public const float Split = 0.01f;
	[Serializable]
	public struct ECSAnimationMakeData
	{
		[SerializeField]
		public int Id;
        [SerializeField]
        public AnimationClip Animation;
        [SerializeField]
        public bool Loop;
        [SerializeField]
        public List<ECSAnimationMakeDataEvent> Events;

        [HideInInspector]
        public int Start;
        [HideInInspector]
        public int End;
        [HideInInspector]
        public float LenSec;
    }

    [Serializable]
    public struct ECSAnimationMakeDataEvent
    {
        [SerializeField]
        public int EventId;
        [SerializeField]
        public float EventTime;
    }

    [SerializeField]
    public List<ECSAnimationMakeData> MakeDatas;
}

