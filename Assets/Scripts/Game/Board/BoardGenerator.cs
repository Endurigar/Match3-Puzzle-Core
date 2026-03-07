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
        private HashSet<Vector2Int> _emptyCellsLookup = new HashSet<Vector2Int>();

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

            _emptyCellsLookup.Clear();
            if (levelData.EmptyCells != null)
            {
                foreach (var cell in levelData.EmptyCells)
                    _emptyCellsLookup.Add(new Vector2Int((int)cell.x, (int)cell.y));
            }

            _board.ResizeBoard(levelData.Width, levelData.Height);
            ClearBoard();

            if (levelData.Obstacles != null)
            {
                foreach (var obs in levelData.Obstacles)
                    SpawnEntity(obs.Prefab, (int)obs.Position.x, (int)obs.Position.y);
            }

            if (levelData.Gems != null)
            {
                foreach (var gem in levelData.Gems)
                    SpawnEntity(gem.Prefab, (int)gem.Position.x, (int)gem.Position.y);
            }

            RegenerateBoardUntilPlayable();
        }

        public void RegenerateBoardUntilPlayable(PossibleMoveChecker checker = null)
        {
            if (_holder == null) return;

            GemType?[,] validLayout = TryGenerateValidLayout(100);

            if (validLayout == null)
            {
                validLayout = GenerateRandomLayout(true);
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
            if (IsCellBlocked(x, y)) return null;
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

        private GemType?[,] TryGenerateValidLayout(int maxTries)
        {
            for (int i = 0; i < maxTries; i++)
            {
                GemType?[,] layout = GenerateRandomLayout(true);
                if (HasVirtualPossibleMove(layout)) return layout;
            }
            return null;
        }

        private GemType?[,] GenerateRandomLayout(bool resolveMatches)
        {
            int w = _board.Width;
            int h = _board.Height;
            GemType?[,] layout = new GemType?[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (IsCellBlocked(x, y))
                    {
                        layout[x, y] = null;
                        continue;
                    }

                    GemType type = GetRandomType();
                    if (resolveMatches)
                    {
                        int safety = 0;
                        while (HasMatchAt(layout, x, y, type) && _availableGemTypes.Count > 1 && safety < 50)
                        {
                            type = GetNextType(type);
                            safety++;
                        }
                    }
                    layout[x, y] = type;
                }
            }
            return layout;
        }

        public bool IsCellBlocked(int x, int y)
        {
            if (_emptyCellsLookup.Contains(new Vector2Int(x, y))) return true;

            var entity = _board.GetGem(x, y);

            if (entity != null && entity is ObstacleController)
            {
                return true;
            }

            return false;
        }

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

        private void ApplyLayoutToBoard(GemType?[,] layout)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    if (layout[x, y].HasValue && _board.GetGem(x, y) == null)
                    {
                        SpawnGemFromPool(layout[x, y].Value, x, y, true);
                    }
                }
            }
        }

        private bool HasVirtualPossibleMove(GemType?[,] layout)
        {
            int w = layout.GetLength(0);
            int h = layout.GetLength(1);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (!layout[x, y].HasValue) continue;

                    if (CheckSwap(layout, x, y, x + 1, y)) return true;
                    if (CheckSwap(layout, x, y, x, y + 1)) return true;
                }
            }
            return false;
        }

        private bool CheckSwap(GemType?[,] layout, int x1, int y1, int x2, int y2)
        {
            int w = layout.GetLength(0);
            int h = layout.GetLength(1);

            if (x2 >= w || y2 >= h || !layout[x2, y2].HasValue) return false;

            GemType t1 = layout[x1, y1].Value;
            GemType t2 = layout[x2, y2].Value;

            layout[x1, y1] = t2;
            layout[x2, y2] = t1;

            bool hasMatch = CheckMatchAt(layout, x1, y1) || CheckMatchAt(layout, x2, y2);

            layout[x1, y1] = t1;
            layout[x2, y2] = t2;

            return hasMatch;
        }

        private bool CheckMatchAt(GemType?[,] layout, int x, int y)
        {
            if (!layout[x, y].HasValue) return false;
            GemType type = layout[x, y].Value;

            int countH = 1;
            for (int i = x - 1; i >= 0 && layout[i, y] == type; i--) countH++;
            for (int i = x + 1; i < layout.GetLength(0) && layout[i, y] == type; i++) countH++;
            if (countH >= 3) return true;

            int countV = 1;
            for (int i = y - 1; i >= 0 && layout[x, i] == type; i--) countV++;
            for (int i = y + 1; i < layout.GetLength(1) && layout[x, i] == type; i++) countV++;
            return countV >= 3;
        }

        private bool HasMatchAt(GemType?[,] layout, int x, int y, GemType current)
        {
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