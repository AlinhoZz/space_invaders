using Avalonia.Controls;
using Avalonia.Input;
using SpaceInvaders.Services;
using System;
using System.Threading.Tasks;

namespace SpaceInvaders.Controllers
{
    public class GameController
    {
        private PlayerController  _playerController;
        private EnemyController   _enemyController;
        private UFOController     _ufoController;
        private BunkerController  _bunkerController;
        private ShotsController   _shotsController;
        private ScoreController   _scoreController;

        private SoundManager _soundManager = new SoundManager();
        
        public bool IsGamePaused { get; private set; } = false;
        
        private bool _waveResetInProgress = false;

        public GameController(Canvas gameCanvas,
                              Image playerShip,
                              TextBlock scoreText,
                              Image life1,
                              Image life2,
                              Image life3)
        {
            _scoreController = new ScoreController(this, scoreText);

            _playerController = new PlayerController(
                gameCanvas, 
                playerShip, 
                life1, life2, life3,
                _soundManager, 
                _scoreController,
                this
            );

            _enemyController = new EnemyController(gameCanvas, _soundManager, _scoreController);
            _bunkerController = new BunkerController(gameCanvas);
            _ufoController = new UFOController(gameCanvas, _soundManager, _scoreController, this);

            _shotsController = new ShotsController(
                gameCanvas,
                _playerController,
                _enemyController,
                _bunkerController,
                _ufoController,
                _soundManager,
                _scoreController,
                this
            );
            
            _scoreController.OnRestartRequested += RestartGame;
        }
        
        public void OnGameLoop()
        {
            if (IsGamePaused) return;

            _playerController.Update();
            _enemyController.Update();
            _ufoController.Update();
            _shotsController.Update();

            _scoreController.CheckGameStatus(_playerController, _enemyController);

            if (_enemyController.Enemies.Count == 0 
                && !_waveResetInProgress
                && !IsGamePaused
                && !_scoreController.AlreadyGameOverOrWin) 
            {
                ResetWave();
            }
        }

        private async void ResetWave()
        {
            _waveResetInProgress = true;
            
            _enemyController.IncreaseSpeed(0.4);
            
            await Task.Delay(1000);

            _enemyController.CreateEnemies();

            _waveResetInProgress = false;
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (IsGamePaused) return;
            _playerController.HandleKeyDown(e);
            _shotsController.HandleKeyDown(e);
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            if (IsGamePaused) return;
            _playerController.HandleKeyUp(e);
        }
        
        public void IncreaseEnemySpeed()
        {
            _enemyController.IncreaseSpeed(0.1);
        }
        
        public void PauseGame()
        {
            IsGamePaused = true;
            _ufoController.PauseUFOAudio();
        }
        
        public void ResumeGame()
        {
            IsGamePaused = false;
            _ufoController.ResumeUFOAudio();
        }
        
        public void PauseAndRemoveAll()
        {
            IsGamePaused = true;

            _ufoController.RemoveUFOCompletely();

            _enemyController.ClearEnemies();

            _bunkerController.ClearAllBunkers();

            _shotsController.ClearAllShots();

            _playerController.HidePlayerCompletely();

            _scoreController.HideScore();
        }


        private void RestartGame()
        {
            IsGamePaused = false;
            _waveResetInProgress = false;

            _scoreController.Restart();
            _playerController.Restart();
            _enemyController.Restart();
            _ufoController.Restart();
            _bunkerController.Restart();
            _shotsController.Restart();
        }
    }
}
