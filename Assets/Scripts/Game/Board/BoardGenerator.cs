using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Board
{
    public class BoardGenerator
    {
        private readonly BoardState _board;
        private readonly GemsScriptableObject _gemsScriptableObject;
        private readonly GemPoolManager _poolManager;
        private Transform _holder;
        private List<GemType> _availableGemTypes;

        public BoardGenerator(BoardState board, GemsScriptableObject gemsScriptableObject,
                              Transform holder, GemPoolManager poolManager)
        {
            _board = board;
            _gemsScriptableObject = gemsScriptableObject;
            _holder = holder;
            _poolManager = poolManager;
            InitializeAvailableTypes();
        }

        public void SetHolder(Transform holder) => _holder = holder;

        public void GenerateLevel(LevelData levelData)
        {
            if (_holder == null) return;

            _board.ResizeBoard(levelData.Width, levelData.Height);
            ClearBoard();

            if (levelData.Obstacles != null)
                foreach (var obs in levelData.Obstacles) SpawnEntity(obs.Prefab, obs.Position.x, obs.Position.y);

            if (levelData.Gems != null)
                foreach (var gem in levelData.Gems) SpawnEntity(gem.Prefab, gem.Position.x, gem.Position.y);

            RegenerateBoardUntilPlayable();
        }

        public void RegenerateBoardUntilPlayable(PossibleMoveChecker checker = null)
        {
            if (_holder == null) return;

            GemType[,] validLayout = TryGenerateValidLayout();

            if (validLayout == null)
            {
                Debug.LogWarning("BoardGenerator: Could not generate guaranteed layout. Using random.");
                validLayout = GenerateRandomLayout(resolveMatches: true);
            }

            ClearBoardExceptObstacles();
            ApplyLayoutToBoard(validLayout);
        }

        public void RegenerateBoardAnimated()
        {
            if (_holder == null) return;

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var entity = _board.GetGem(x, y);
                    if (entity == null || entity is ObstacleController) continue;

                    var gemEntity = entity;
                    entity.transform.DOScale(Vector3.zero, 0.3f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => _poolManager.Release(gemEntity));

                    _board.SetGem(x, y, null);
                }
            }
        }

        public BoardEntity SpawnGem(int x, int y, bool animate = true)
        {
            return SpawnGemFromPool(GetRandomType(), x, y, animate);
        }

        private BoardEntity SpawnGemFromPool(GemType type, int x, int y, bool animate)
        {
            if (_holder == null) return null;

            var gem = _poolManager.GetGem(type);
            if (gem == null) return null;

            gem.transform.SetParent(_holder);
            gem.transform.localPosition = new Vector3(x, y, 0);

            var prefab = _gemsScriptableObject.GetGemByType(type);
            Vector3 targetScale = prefab != null ? prefab.transform.localScale : Vector3.one;

            if (animate)
            {
                gem.transform.localScale = Vector3.zero;
                gem.transform.DOScale(targetScale, 0.4f).SetEase(Ease.OutBack);
            }
            else
            {
                gem.transform.localScale = targetScale;
            }

            gem.GridPosition = new Vector2(x, y);
            _board.SetGem(x, y, gem);
            return gem;
        }

        private void SpawnEntity(GameObject prefab, int x, int y)
        {
            if (_holder == null || prefab == null) return;

            var entityGO = Object.Instantiate(prefab, _holder);
            entityGO.transform.localPosition = new Vector3(x, y, 0);

            BoardEntity controller = entityGO.GetComponent<BoardEntity>();
            controller.GridPosition = new Vector2(x, y);
            _board.SetGem(x, y, controller);
        }

        private GemType[,] TryGenerateValidLayout(int maxTries = 50)
        {
            for (int i = 0; i < maxTries; i++)
            {
                GemType[,] layout = GenerateRandomLayout(resolveMatches: true);
                if (HasVirtualPossibleMove(layout)) return layout;
            }
            return null;
        }

        private GemType[,] GenerateRandomLayout(bool resolveMatches)
        {
            int w = _board.Width;
            int h = _board.Height;
            GemType[,] layout = new GemType[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (IsObstacle(x, y))
                    {
                        layout[x, y] = GemType.Red; // Placeholder
                    }
                    else
                    {
                        layout[x, y] = GetRandomType();
                        if (resolveMatches)
                        {
                            while (HasMatchAt(layout, x, y) && _availableGemTypes.Count > 1)
                            {
                                layout[x, y] = GetNextType(layout[x, y]);
                            }
                        }
                    }
                }
            }
            return layout;
        }

        private bool IsObstacle(int x, int y) => _board.GetGem(x, y) is ObstacleController;

        private void ClearBoard()
        {
            if (_holder == null) return;
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var gem = _board.GetGem(x, y);
                    if (gem != null) _poolManager.Release(gem);
                    _board.SetGem(x, y, null);
                }
            }
        }

        private void ClearBoardExceptObstacles()
        {
            if (_holder == null) return;
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var entity = _board.GetGem(x, y);
                    if (entity != null && !(entity is ObstacleController))
                    {
                        _poolManager.Release(entity);
                        _board.SetGem(x, y, null);
                    }
                }
            }
        }

        private void ApplyLayoutToBoard(GemType[,] layout)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    if (_board.GetGem(x, y) == null)
                    {
                        SpawnGemFromPool(layout[x, y], x, y, true);
                    }
                }
            }
        }

        private bool HasVirtualPossibleMove(GemType[,] layout)
        {
            int w = _board.Width;
            int h = _board.Height;
            int[] dx = { 1, 0 };
            int[] dy = { 0, 1 };

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (IsObstacle(x, y)) continue;

                    for (int i = 0; i < 2; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (nx >= w || ny >= h || IsObstacle(nx, ny)) continue;

                        GemType t1 = layout[x, y];
                        GemType t2 = layout[nx, ny];

                        layout[x, y] = t2;
                        layout[nx, ny] = t1;

                        bool hasMatch = CheckMatchAt(layout, x, y) || CheckMatchAt(layout, nx, ny);

                        layout[x, y] = t1;
                        layout[nx, ny] = t2;

                        if (hasMatch) return true;
                    }
                }
            }
            return false;
        }

        private bool CheckMatchAt(GemType[,] layout, int x, int y)
        {
            GemType type = layout[x, y];
            int w = _board.Width;
            int h = _board.Height;

            int countH = 1;
            for (int i = x - 1; i >= 0 && layout[i, y] == type && !IsObstacle(i, y); i--) countH++;
            for (int i = x + 1; i < w && layout[i, y] == type && !IsObstacle(i, y); i++) countH++;

            if (countH >= 3) return true;

            int countV = 1;
            for (int i = y - 1; i >= 0 && layout[x, i] == type && !IsObstacle(x, i); i--) countV++;
            for (int i = y + 1; i < h && layout[x, i] == type && !IsObstacle(x, i); i++) countV++;

            return countV >= 3;
        }

        private bool HasMatchAt(GemType[,] layout, int x, int y)
        {
            GemType current = layout[x, y];
            if (x >= 2 && layout[x - 1, y] == current && layout[x - 2, y] == current) return true;
            if (y >= 2 && layout[x, y - 1] == current && layout[x, y - 2] == current) return true;
            return false;
        }

        private void InitializeAvailableTypes()
        {
            _availableGemTypes = new List<GemType>();
            foreach (GemType type in Enum.GetValues(typeof(GemType)))
            {
                if (_gemsScriptableObject.GetGemByType(type) != null)
                    _availableGemTypes.Add(type);
            }
        }

        private GemType GetRandomType()
        {
            if (_availableGemTypes == null || _availableGemTypes.Count == 0) return GemType.Red;
            return _availableGemTypes[Random.Range(0, _availableGemTypes.Count)];
        }

        private GemType GetNextType(GemType current)
        {
            int index = _availableGemTypes.IndexOf(current);
            index = (index + 1) % _availableGemTypes.Count;
            return _availableGemTypes[index];
        }
    }
}