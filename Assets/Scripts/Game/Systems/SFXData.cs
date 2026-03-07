using System;
using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    [Serializable]
    public class SFXData
    {
        public SFXType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
    }

    [Serializable]
    public class MusicData
    {
        public MusicType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 0.5f;
    }
}