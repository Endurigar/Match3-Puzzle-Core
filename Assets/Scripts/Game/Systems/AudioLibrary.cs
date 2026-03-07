using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Match3/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        [SerializeField] private List<SFXData> _sfxList;
        [SerializeField] private List<MusicData> _musicList;

        public SFXData GetSFX(SFXType type)
        {
            return _sfxList?.FirstOrDefault(x => x.Type == type);
        }

        public MusicData GetMusic(MusicType type)
        {
            return _musicList?.FirstOrDefault(x => x.Type == type);
        }
    }
}