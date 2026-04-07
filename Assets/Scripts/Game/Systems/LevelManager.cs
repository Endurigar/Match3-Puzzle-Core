using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Manages level selection and provides access to the currently selected level data across scenes.
    /// Implementation follows the Singleton pattern.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the singleton instance of the LevelManager.
        /// </summary>
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelData _selectedLevelData;

        /// <summary>
        /// Gets the currently selected level data.
        /// </summary>
        public LevelData SelectedLevelData => _selectedLevelData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Sets the specified level data as selected.
        /// </summary>
        /// <param name="levelData">The data of the level to select.</param>
        public void SelectLevel(LevelData levelData)
        {
            _selectedLevelData = levelData;
        }
    }
}
 village
 village