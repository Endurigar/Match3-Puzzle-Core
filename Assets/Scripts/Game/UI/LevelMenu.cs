using System.Collections.Generic;
using Assets.Scripts.Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Game.UI
{
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

        [Inject]
        public void Construct(LevelProgressManager progressManager)
        {
            _progressManager = progressManager;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(OnLevelButtonClicked);
        }

        public void SetLevelInfo(LevelData levelInfo)
        {
            _levelData = levelInfo;
            if (_levelNameText) _levelNameText.text = _levelData.Name;

            UpdateVisuals();
        }

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