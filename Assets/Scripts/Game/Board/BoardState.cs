using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;

namespace Assets.Scripts.Game.Board
{
    public class BoardState : IInitializable
    {
        private BoardEntity[,] _grid;

        private readonly BoardGenerator _boardGenerator;
        private readonly MatchResolver _matchResolver;
        private readonly PowerupProcessor _powerupProcessor;
        private readonly GravityController _gravityController;
        private readonly MatchFinder _matchFinder;
        private readonly PossibleMoveChecker _possibleMoveChecker;

        private readonly HintSystem _hintSystem;
        private readonly VFXManager _vfxManager;
        private readonly LevelData _currentLevelData;
        private readonly Transform _holder;

        private bool _isCheckingMatches = false;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public PossibleMoveChecker PossibleMoveChecker => _possibleMoveChecker;

        [Inject]
        public BoardState(
            GemsScriptableObject gemsScriptableObject,
            MatchAnimator matchAnimator,
            LevelData levelData,
            ScoreManager scoreManager,
            [Inject(Id = "Holder")] Transform holder,
            VFXManager vfxManager,
            AudioManager audioManager,
            FloatingTextManager floatingTextManager,
            CameraEffectManager cameraEffectManager,
            HintSystem hintSystem,
            GemPoolManager gemPoolManager)
        {
            _holder = holder;
            _hintSystem = hintSystem;
            _vfxManager = vfxManager;
            _currentLevelData = levelData;
            _grid = new BoardEntity[0, 0];

            _boardGenerator = new BoardGenerator(this, gemsScriptableObject, holder, gemPoolManager);
            _gravityController = new GravityController(this, _boardGenerator);
            _matchResolver = new MatchResolver(this, _gravityController, matchAnimator, gemsScriptableObject, scoreManager, vfxManager, audioManager, floatingTextManager, gemPoolManager);
            _powerupProcessor = new PowerupProcessor(this, matchAnimator, scoreManager, vfxManager, audioManager, floatingTextManager, cameraEffectManager, gemPoolManager);

            _matchFinder = new MatchFinder(this);
            _possibleMoveChecker = new PossibleMoveChecker(this, _matchFinder);

            _hintSystem.Initialize(_possibleMoveChecker, _vfxManager);
        }

        public async void Initialize()
        {
            if (_currentLevelData == null) return;

            _boardGenerator.SetHolder(_holder);
            _boardGenerator.GenerateLevel(_currentLevelData);
            await CheckMatches();
        }

        public void ResizeBoard(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new BoardEntity[Width, Height];
        }

        public BoardEntity GetGem(int x, int y) => IsInside(new Vector2(x, y)) ? _grid[x, y] : null;

        public void SetGem(int x, int y, BoardEntity gem)
        {
            if (!IsInside(new Vector2(x, y))) return;
            _grid[x, y] = gem;
            if (gem != null) gem.GridPosition = new Vector2(x, y);
        }

        public void SetGem(float x, float y, BoardEntity gem) => SetGem((int)x, (int)y, gem);

        public bool IsInside(Vector2 pos) => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;

        public bool HasMatches() => _matchResolver.HasMatches();

        public async Task CheckMatches()
        {
            if (_isCheckingMatches) return;
            _isCheckingMatches = true;

            await _matchResolver.ResolveMatches();

            if (!_possibleMoveChecker.HasPossibleMoves())
            {
                _boardGenerator.RegenerateBoardAnimated();
                await Task.Delay(500);
                _boardGenerator.RegenerateBoardUntilPlayable();
                await CheckMatches();
            }

            _isCheckingMatches = false;
        }

        public List<BoardEntity> GetAllGems()
        {
            var gems = new List<BoardEntity>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_grid[x, y] != null) gems.Add(_grid[x, y]);
                }
            }
            return gems;
        }
    }
}