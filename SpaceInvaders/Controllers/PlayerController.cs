using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using SpaceInvaders.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpaceInvaders.Controllers
{
    public class PlayerController
    {
        private readonly Canvas _gameCanvas;
        private readonly Image _playerShip;
        private readonly Image _life1, _life2, _life3;
        private readonly SoundManager _soundManager;
        private readonly ScoreController _scoreController;
        private readonly GameController _gameController;
        
        public int Lives { get; private set; } = 3;
        private double _shipX = 380;
        private const double ShipWidth = 40;
        private const double PlayerMoveSpeed = 6.0;
        private int _moveDirection = 0;
        
        private Bitmap[] _playerExplodingFrames;
        private const double PlayerDeathAnimationDuration = 1500;
        private const int PlayerDeathAnimationToggles = 6;

        public PlayerController(Canvas gameCanvas,
                                Image playerShip,
                                Image life1,
                                Image life2,
                                Image life3,
                                SoundManager soundManager,
                                ScoreController scoreController,
                                GameController gameController)
        {
            _gameCanvas = gameCanvas;
            _playerShip = playerShip;
            _life1 = life1;
            _life2 = life2;
            _life3 = life3;
            _soundManager = soundManager;
            _scoreController = scoreController;
            _gameController = gameController;

            _playerExplodingFrames = LoadExplodingFrames();
            UpdateLifeSprites();
            Canvas.SetLeft(_playerShip, _shipX);
        }

        private Bitmap[] LoadExplodingFrames()
        {
            var path = AppContext.BaseDirectory;
            var spritesPath = Path.Combine(path, "Assets", "sprites");
            return new Bitmap[]
            {
                new Bitmap(Path.Combine(spritesPath, "PlayerExploding1.png")),
                new Bitmap(Path.Combine(spritesPath, "PlayerExploding2.png"))
            };
        }

        public void Update()
        {

            double delta = _moveDirection * PlayerMoveSpeed;
            MoveShip(delta);
        }

        private void MoveShip(double delta)
        {
            double newX = _shipX + delta;
            if (newX < 0) newX = 0;
            if (newX > 800 - ShipWidth) newX = 800 - ShipWidth;
            _shipX = newX;
            Canvas.SetLeft(_playerShip, _shipX);
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _moveDirection = -1;
                    break;
                case Key.Right:
                    _moveDirection = 1;
                    break;
            }
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (_moveDirection == -1) _moveDirection = 0;
                    break;
                case Key.Right:
                    if (_moveDirection == 1) _moveDirection = 0;
                    break;
            }
        }

        public async void TakeHit()
        {
            if (Lives <= 0) return;

            _soundManager.PlaySound("explosion.wav");
            Lives--;
            UpdateLifeSprites();

            _gameController.PauseGame();

            double interval = PlayerDeathAnimationDuration / PlayerDeathAnimationToggles;
            for (int i = 0; i < PlayerDeathAnimationToggles; i++)
            {
                _playerShip.Source = (i % 2 == 0)
                    ? _playerExplodingFrames[0]
                    : _playerExplodingFrames[1];
                await Task.Delay((int)interval);
            }

            if (Lives <= 0)
            {
                _scoreController.GameOver(); 
                return;
            }

            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            _playerShip.Source = new Bitmap(Path.Combine(spritesPath, "4.png"));
            _shipX = 380;
            Canvas.SetLeft(_playerShip, _shipX);

            _gameController.ResumeGame();
        }

        public void AddLife(int amount = 1)
        {
            Lives += amount;
            UpdateLifeSprites();
        }

        private void UpdateLifeSprites()
        {
            if (Lives >= 1)
            {
                if (!_gameCanvas.Children.Contains(_life1))
                    _gameCanvas.Children.Add(_life1);
            }
            else
            {
                _gameCanvas.Children.Remove(_life1);
            }
            
            if (Lives >= 2)
            {
                if (!_gameCanvas.Children.Contains(_life2))
                    _gameCanvas.Children.Add(_life2);
            }
            else
            {
                _gameCanvas.Children.Remove(_life2);
            }
            
            if (Lives >= 3)
            {
                if (!_gameCanvas.Children.Contains(_life3))
                    _gameCanvas.Children.Add(_life3);
            }
            else
            {
                _gameCanvas.Children.Remove(_life3);
            }
        }

        public double GetShipLeft() => _shipX;
        public double GetShipWidth() => ShipWidth;
        public Image GetPlayerImage() => _playerShip;
        
        public void HidePlayerCompletely()
        {
            _playerShip.IsVisible = false;
            _gameCanvas.Children.Remove(_life1);
            _gameCanvas.Children.Remove(_life2);
            _gameCanvas.Children.Remove(_life3);
        }

        public void Restart()
        {
            Lives = 3;
            _moveDirection = 0;
            _shipX = 380;

            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            _playerShip.Source = new Bitmap(Path.Combine(spritesPath, "4.png"));
            _playerShip.IsVisible = true;
            Canvas.SetLeft(_playerShip, _shipX);

            UpdateLifeSprites();
        }
    }
}
