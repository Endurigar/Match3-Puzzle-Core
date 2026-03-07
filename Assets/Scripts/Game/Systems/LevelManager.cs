using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelData _selectedLevelData;
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

        public void SelectLevel(LevelData levelData)
        {
            _selectedLevelData = levelData;
        }
    }
}