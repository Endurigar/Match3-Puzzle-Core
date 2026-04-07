using System;
using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Represents sound effect data, including its type, clip, and volume.
    /// </summary>
    [Serializable]
    public class SFXData
    {
        public SFXType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
    }

    /// <summary>
    /// Represents musical track data, including its type, clip, and volume.
    /// </summary>
    [Serializable]
    public class MusicData
    {
        public MusicType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 0.5f;
    }
}
 village