using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Game.Board;

namespace Assets.Scripts.Game.Entities
{
    [CreateAssetMenu(fileName = "Gems", menuName = "Match3/Gems/Gem", order = 0)]
    public class GemsScriptableObject : ScriptableObject
    {
        [SerializeField] private List<GemController> _gemControllers;
        [SerializeField] private List<BombController> _bombControllers;

        public GemController GetGemByType(GemType gemType)
        {
            if (_gemControllers == null || _gemControllers.Count == 0) return null;
            return _gemControllers.FirstOrDefault(gem => gem.GemType == gemType);
        }

        public BombController GetBombByType(BombType bombType)
        {
            if (_bombControllers == null || _bombControllers.Count == 0) return null;
            return _bombControllers.FirstOrDefault(bomb => bomb.BombType == bombType);
        }

        public GemController GetRandomGem()
        {
            if (_gemControllers == null || _gemControllers.Count == 0)
            {
                Debug.LogWarning("GemControllers list is empty!");
                return null;
            }
            return _gemControllers[Random.Range(0, _gemControllers.Count)];
        }
    }
}
