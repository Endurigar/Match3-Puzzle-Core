// UI/LevelMenu.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelName;
        [SerializeField] private Button levelButton;
        private LevelData levelData;

        public void SetLevelInfo(LevelData levelInfo)
        {
            this.levelData = levelInfo;
            levelName.text = this.levelData.Name;
            levelButton.onClick.AddListener(LevelButtonClicked);
        }

        private void LevelButtonClicked()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.SelectLevel(levelData);
            }
            SceneManager.LoadScene("GameScene"); // Загружаем одну и ту же сцену для всех уровней
        }
    }
}