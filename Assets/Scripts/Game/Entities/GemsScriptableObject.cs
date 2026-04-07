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

        /// <summary>
        /// Retrieves the GemController prefab for a specific gem type.
        /// </summary>
        /// <param name="gemType">The type of gem to find.</param>
        /// <returns>The GemController prefab, or null if not found.</returns>
        public GemController GetGemByType(GemType gemType)
        {
            if (_gemControllers == null || _gemControllers.Count == 0) return null;
            return _gemControllers.FirstOrDefault(gem => gem.GemType == gemType);
        }

        /// <summary>
        /// Retrieves the BombController prefab for a specific bomb type.
        /// </summary>
        /// <param name="bombType">The type of bomb to find.</param>
        /// <returns>The BombController prefab, or null if not found.</returns>
        public BombController GetBombByType(BombType bombType)
        {
            if (_bombControllers == null || _bombControllers.Count == 0) return null;
            return _bombControllers.FirstOrDefault(bomb => bomb.BombType == bombType);
        }

        /// <summary>
        /// Retrieves a random GemController prefab from the available list.
        /// </summary>
        /// <returns>A random GemController prefab, or null if the list is empty.</returns>
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
