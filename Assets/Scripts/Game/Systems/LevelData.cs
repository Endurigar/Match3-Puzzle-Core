using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Systems
{
    [System.Serializable]
    public class ObstaclePlacement
    {
        public Vector2Int Position;
        public GameObject Prefab;
    }

    [System.Serializable]
    public class GemPlacement
    {
        public Vector2Int Position;
        public GameObject Prefab;
    }

    [CreateAssetMenu(fileName = "New LevelData", menuName = "Match3/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string Name;
        public string SceneName;
        public int Width = 5;
        public int Height = 5;

        [Header("Progression")]
        public LevelData NextLevel;

        [Header("Gameplay Settings")]
        public float TimeLimit = 60f;
        public int TargetScore = 1000;
        public bool IsEndlessMode = false;

        [Header("Board Setup")]
        public List<ObstaclePlacement> Obstacles;
        public List<GemPlacement> Gems;
        public List<Vector2Int> EmptyCells;
    }
}