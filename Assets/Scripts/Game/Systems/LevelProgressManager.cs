using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    public class LevelProgressManager
    {
        public void SaveLevelStars(string levelName, int stars)
        {
            Debug.Log($"[Progress] Saving Stars for {levelName}: {stars}");

            int currentStars = GetLevelStars(levelName);

            if (stars > currentStars)
            {
                PlayerPrefs.SetInt($"Level_{levelName}_Stars", stars);
                PlayerPrefs.Save();
            }
        }

        public int GetLevelStars(string levelName)
        {
            return PlayerPrefs.GetInt($"Level_{levelName}_Stars", 0);
        }

        public void UnlockLevel(string levelName)
        {
            Debug.Log($"[Progress] Unlocking Level: {levelName}");
            if (!IsLevelUnlocked(levelName))
            {
                PlayerPrefs.SetInt($"Level_{levelName}_Unlocked", 1);
                PlayerPrefs.Save();
            }
        }

        public bool IsLevelUnlocked(string levelName)
        {
            if (levelName == "Level 1" || levelName == "Level1") return true;

            return PlayerPrefs.GetInt($"Level_{levelName}_Unlocked", 0) == 1;
        }

        public void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}