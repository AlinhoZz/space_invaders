// Controllers/UFOController.cs
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using SpaceInvaders.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpaceInvaders.Controllers
{
    public class UFOController
    {
        private readonly Canvas _gameCanvas;
        private readonly SoundManager _soundManager;
        private readonly ScoreController _scoreController;
        private readonly GameController _gameController;

        private Image? _ufo;
        private bool _ufoMovingLeft;
        private bool _ufoHit = false;
        private double _ufoSpawnTimer = 0;
        private double _ufoSpawnInterval = 0;
        private const double _ufoSpeed = 3;
        private bool _ufoSoundPlaying = false;

        public UFOController(Canvas gameCanvas,
                             SoundManager soundManager,
                             ScoreController scoreController,
                             GameController gameController)
        {
            _gameCanvas = gameCanvas;
            _soundManager = soundManager;
            _scoreController = scoreController;
            _gameController = gameController;

            var rnd = new Random();
            _ufoSpawnInterval = rnd.Next(5000, 15000);
        }

        public void Update()
        {
            if (_gameController.IsGamePaused) return;

            if (_ufo != null && !_ufoHit)
            {
                double left = Canvas.GetLeft(_ufo);
                if (_ufoMovingLeft)
                {
                    left -= _ufoSpeed;
                    Canvas.SetLeft(_ufo, left);
                    if (left + _ufo.Width < 0)
                        DespawnUFO();
                }
                else
                {
                    left += _ufoSpeed;
                    Canvas.SetLeft(_ufo, left);
                    if (left > 800)
                        DespawnUFO();
                }
            }

            _ufoSpawnTimer += 16;
            if (_ufo == null && _ufoSpawnTimer >= _ufoSpawnInterval)
            {
                SpawnUFO();
                _ufoSpawnTimer = 0;
                var rnd = new Random();
                _ufoSpawnInterval = rnd.Next(5000, 15000);
            }
        }

        private void SpawnUFO()
        {
            var sp = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            var path = Path.Combine(sp, "AlienShip.png");
            _ufo = new Image
            {
                Source = new Bitmap(path),
                Width = 50,
                Height = 30
            };
            _ufoHit = false;

            var rnd = new Random();
            _ufoMovingLeft = (rnd.Next(2) == 0);

            if (!_ufoMovingLeft)
                Canvas.SetLeft(_ufo, -_ufo.Width);
            else
                Canvas.SetLeft(_ufo, 800);

            Canvas.SetTop(_ufo, 20);
            _gameCanvas.Children.Add(_ufo);

            StartUFOSound();
        }

        private void DespawnUFO()
        {
            StopUFOSound();
            if (_ufo != null)
            {
                _gameCanvas.Children.Remove(_ufo);
                _ufo = null;
            }
            _ufoHit = false;
            _ufoSpawnTimer = 0;
            var rnd = new Random();
            _ufoSpawnInterval = rnd.Next(5000, 15000);
        }

        public async Task UFOHit(int points)
        {
            if (_ufo == null) return;

            _ufoHit = true;
            StopUFOSound();
            _soundManager.PlaySound("ufo_highpitch.wav");

            _scoreController.AddScore(points);
            _scoreController.ShowFloatingText($"+{points}", Canvas.GetLeft(_ufo) + 15, Canvas.GetTop(_ufo));

            for (int i = 0; i < 5; i++)
            {
                _ufo.IsVisible = !_ufo.IsVisible;
                await Task.Delay(100);
                if (_ufo == null) return;
            }
            _ufo.IsVisible = true;
            DespawnUFO();
        }

        private void StartUFOSound()
        {
            if (!_ufoSoundPlaying)
            {
                _soundManager.StartUFOLoop("ufo_lowpitch.wav");
                _ufoSoundPlaying = true;
            }
        }

        private void StopUFOSound()
        {
            _soundManager.StopUFOLoop();
            _ufoSoundPlaying = false;
        }

        public void PauseUFOAudio()
        {
            if (_ufoSoundPlaying)
            {
                StopUFOSound();
            }
        }

        public void ResumeUFOAudio()
        {
            if (_ufo != null && !_ufoHit)
            {
                StartUFOSound();
            }
        }

        public void RemoveUFOCompletely()
        {
            StopUFOSound();
            if (_ufo != null)
            {
                _gameCanvas.Children.Remove(_ufo);
                _ufo = null;
            }
            _ufoHit = false;
        }

        public Image? GetUFO() => _ufo;

        public void Restart()
        {
            RemoveUFOCompletely();
            _ufoSpawnTimer = 0;
            var rnd = new Random();
            _ufoSpawnInterval = rnd.Next(5000, 15000);
        }
    }
}
