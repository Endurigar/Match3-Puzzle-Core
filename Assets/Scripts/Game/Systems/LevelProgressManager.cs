using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    /// <summary>
    /// Manages saving and loading of level progress, including stars and unlock status.
    /// </summary>
    public class LevelProgressManager
    {
        /// <summary>
        /// Saves the number of stars earned for a specific level if it exceeds the current record.
        /// </summary>
        /// <param name="levelName">The unique name of the level.</param>
        /// <param name="stars">The number of stars earned (0-3).</param>
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

        /// <summary>
        /// Retrieves the number of stars earned for a specific level.
        /// </summary>
        /// <param name="levelName">The unique name of the level.</param>
        /// <returns>The number of stars earned, or 0 if no progress exists.</returns>
        public int GetLevelStars(string levelName)
        {
            return PlayerPrefs.GetInt($"Level_{levelName}_Stars", 0);
        }

        /// <summary>
        /// Unlocks a specific level for play.
        /// </summary>
        /// <param name="levelName">The unique name of the level to unlock.</param>
        public void UnlockLevel(string levelName)
        {
            Debug.Log($"[Progress] Unlocking Level: {levelName}");
            if (!IsLevelUnlocked(levelName))
            {
                PlayerPrefs.SetInt($"Level_{levelName}_Unlocked", 1);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Checks if a specific level is unlocked.
        /// </summary>
        /// <param name="levelName">The unique name of the level.</param>
        /// <returns>True if the level is unlocked, false otherwise.</returns>
        public bool IsLevelUnlocked(string levelName)
        {
            if (levelName == "Level 1" || levelName == "Level1") return true;

            return PlayerPrefs.GetInt($"Level_{levelName}_Unlocked", 0) == 1;
        }

        /// <summary>
        /// Resets all player progress saved in PlayerPrefs.
        /// </summary>
        public void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}