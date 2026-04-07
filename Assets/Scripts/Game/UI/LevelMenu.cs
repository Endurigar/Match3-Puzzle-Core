using System.Collections.Generic;
using Assets.Scripts.Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// Represents a level selection item in the UI.
    /// Displays level name, unlock status, and earned stars.
    /// </summary>
    public class LevelMenu : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Text _levelNameText;
        [SerializeField] private Button _playButton;
        [SerializeField] private GameObject _lockIcon;

        [Header("Stars Visuals")]
        [SerializeField] private List<Image> _starImages;
        [SerializeField] private Sprite _starFilled;
        [SerializeField] private Sprite _starEmpty;

        private LevelData _levelData;
        private LevelProgressManager _progressManager;

        /// <summary>
        /// Injects the level progress manager dependency.
        /// </summary>
        [Inject]
        public void Construct(LevelProgressManager progressManager)
        {
            _progressManager = progressManager;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(OnLevelButtonClicked);
        }

        /// <summary>
        /// Configures the menu item with specific level data and updates its visuals.
        /// </summary>
        /// <param name="levelInfo">The data for the level this menu item represents.</param>
        public void SetLevelInfo(LevelData levelInfo)
        {
            _levelData = levelInfo;
            if (_levelNameText) _levelNameText.text = _levelData.Name;

            UpdateVisuals();
        }

        /// <summary>
        /// Updates the interactability, lock icon, and star display based on player progress.
        /// </summary>
        private void UpdateVisuals()
        {
            if (_progressManager == null) return;

            bool isUnlocked = _progressManager.IsLevelUnlocked(_levelData.Name);
            int stars = _progressManager.GetLevelStars(_levelData.Name);

            _playButton.interactable = isUnlocked;

            if (_lockIcon != null)
                _lockIcon.SetActive(!isUnlocked);

            UpdateStars(isUnlocked, stars);
        }

        /// <summary>
        /// Updates the star images to reflect the number of stars earned on this level.
        /// </summary>
        /// <param name="isUnlocked">Whether the level is unlocked.</param>
        /// <param name="earnedStars">The number of stars earned (0-3).</param>
        private void UpdateStars(bool isUnlocked, int earnedStars)
        {
            if (!isUnlocked)
            {
                foreach (var star in _starImages) star.gameObject.SetActive(false);
                return;
            }

            for (int i = 0; i < _starImages.Count; i++)
            {
                _starImages[i].gameObject.SetActive(true);
                _starImages[i].sprite = (i < earnedStars) ? _starFilled : _starEmpty;
            }
        }

        /// <summary>
        /// Handles the level button click event, selects the level, and loads the game scene.
        /// </summary>
        private void OnLevelButtonClicked()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.SelectLevel(_levelData);
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}