using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpaceInvaders.Controllers
{
    public class ShotsController
    {
        private readonly Canvas _gameCanvas;
        private readonly PlayerController _playerController;
        private readonly EnemyController  _enemyController;
        private readonly BunkerController _bunkerController;
        private readonly UFOController    _ufoController;
        private readonly SoundManager _soundManager;
        private readonly ScoreController _scoreController;
        private readonly GameController  _gameController;

        private List<Image> _playerBullets = new();
        private List<Image> _enemyShots    = new();

        private Dictionary<string, Bitmap[]> _alienShotAnimations = new();
        private double _alienShotAnimationTimer = 0;
        private int _currentAlienShotFrame = 0;
        private double _alienShotSpeed  = 5;
        private double _alienShotChance = 0.01;

        public ShotsController(Canvas gameCanvas,
                               PlayerController playerController,
                               EnemyController enemyController,
                               BunkerController bunkerController,
                               UFOController ufoController,
                               SoundManager soundManager,
                               ScoreController scoreController,
                               GameController gameController)
        {
            _gameCanvas      = gameCanvas;
            _playerController= playerController;
            _enemyController = enemyController;
            _bunkerController= bunkerController;
            _ufoController   = ufoController;
            _soundManager    = soundManager;
            _scoreController = scoreController;
            _gameController  = gameController;

            LoadAlienShotAnimations();
        }

        private void LoadAlienShotAnimations()
        {
            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            _alienShotAnimations["MonsterShot1"] = new[]
            {
                new Bitmap(Path.Combine(spritesPath, "MonsterShot1-1.png")),
                new Bitmap(Path.Combine(spritesPath, "MonsterShot1-2.png"))
            };
            _alienShotAnimations["MonsterShot2"] = new[]
            {
                new Bitmap(Path.Combine(spritesPath, "MonsterShot2-1.png")),
                new Bitmap(Path.Combine(spritesPath, "MonsterShot2-2.png"))
            };
        }

        public void Update()
        {
            if (_gameController.IsGamePaused) return;

            UpdatePlayerShots();
            RandomAlienShot();
            UpdateAlienShots();

            _alienShotAnimationTimer += 16;
            if (_alienShotAnimationTimer >= 200)
            {
                _alienShotAnimationTimer = 0;
                _currentAlienShotFrame = 1 - _currentAlienShotFrame;
                foreach (var shot in _enemyShots)
                {
                    if (shot.Tag is string st && _alienShotAnimations.ContainsKey(st))
                    {
                        shot.Source = _alienShotAnimations[st][_currentAlienShotFrame];
                    }
                }
            }
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (_gameController.IsGamePaused) return;

            if (e.Key == Key.Space)
            {
                // Se quiser permitir 1 tiro por vez:
                if (_playerBullets.Count == 0)
                {
                    _soundManager.PlaySound("shoot.wav");
                    FirePlayerShot();
                }
            }
        }

        private void FirePlayerShot()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites", "tiro_user.png");
            var bullet = new Image
            {
                Source = File.Exists(path) ? new Bitmap(path) : null,
                Width = 5,
                Height = 10
            };
            double bx = _playerController.GetShipLeft() + (_playerController.GetShipWidth()/2) - (bullet.Width/2);
            double by = 550 - bullet.Height;
            Canvas.SetLeft(bullet, bx);
            Canvas.SetTop(bullet, by);
            _gameCanvas.Children.Add(bullet);
            _playerBullets.Add(bullet);
        }

        private void UpdatePlayerShots()
        {
            for (int i = _playerBullets.Count - 1; i >= 0; i--)
            {
                var bullet = _playerBullets[i];
                double top = Canvas.GetTop(bullet);
                top -= 10;
                Canvas.SetTop(bullet, top);
                
                for (int j = _enemyController.Enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = _enemyController.Enemies[j];
                    if (IsCollision(bullet, enemy))
                    {
                        _gameCanvas.Children.Remove(bullet);
                        _playerBullets.RemoveAt(i);
                        
                        _ = _enemyController.KillEnemy(enemy);

                        goto bulletDone;
                    }
                }
                
                var bunkers = _gameCanvas.Children
                    .OfType<Image>()
                    .Where(x => x.Tag?.ToString() == "Bunker")
                    .ToList();
                foreach (var b in bunkers)
                {
                    if (IsCollision(bullet, b))
                    {
                        _gameCanvas.Children.Remove(bullet);
                        _playerBullets.RemoveAt(i);

                        _bunkerController.DamageBunker(b);
                        goto bulletDone;
                    }
                }
                
                var ufo = _ufoController.GetUFO();
                if (ufo != null && IsCollision(bullet, ufo))
                {
                    _gameCanvas.Children.Remove(bullet);
                    _playerBullets.RemoveAt(i);

                    var rnd = new Random();
                    int val = (rnd.Next(100) == 0) ? 1000 :
                              new int[] {200,150,100,300,350,400}[rnd.Next(6)];
                    _ = _ufoController.UFOHit(val);

                    goto bulletDone;
                }

                // Saiu da tela
                if (top < 0)
                {
                    _gameCanvas.Children.Remove(bullet);
                    _playerBullets.RemoveAt(i);
                }

            bulletDone:;
            }
        }

        private void RandomAlienShot()
        {
            if (_enemyController.Enemies.Count == 0) return;

            var rnd = new Random();
            if (rnd.NextDouble() < _alienShotChance)
            {
                var randomEnemy = _enemyController.Enemies[rnd.Next(_enemyController.Enemies.Count)];
                double x = Canvas.GetLeft(randomEnemy) + 10;
                double y = Canvas.GetTop(randomEnemy)  + 30;
                string st = (rnd.Next(2) == 0) ? "MonsterShot1" : "MonsterShot2";

                var shot = new Image
                {
                    Source = _alienShotAnimations[st][0],
                    Width = 10,
                    Height = 20,
                    Tag   = st
                };
                Canvas.SetLeft(shot, x);
                Canvas.SetTop(shot, y);
                _gameCanvas.Children.Add(shot);
                _enemyShots.Add(shot);
            }
        }

        private void UpdateAlienShots()
        {
            for (int i = _enemyShots.Count - 1; i >= 0; i--)
            {
                var shot = _enemyShots[i];
                double top = Canvas.GetTop(shot);
                top += _alienShotSpeed;
                Canvas.SetTop(shot, top);

                // Player
                if (IsCollision(shot, _playerController.GetPlayerImage()))
                {
                    _gameCanvas.Children.Remove(shot);
                    _enemyShots.RemoveAt(i);
                    _playerController.TakeHit();
                    continue;
                }

                // Bunkers
                var bunkers = _gameCanvas.Children
                    .OfType<Image>()
                    .Where(x => x.Tag?.ToString() == "Bunker")
                    .ToList();
                foreach (var b in bunkers)
                {
                    if (IsCollision(shot, b))
                    {
                        _gameCanvas.Children.Remove(shot);
                        _enemyShots.RemoveAt(i);
                        _bunkerController.DamageBunker(b);
                        break;
                    }
                }

                // Saiu da tela
                if (top > 600)
                {
                    _gameCanvas.Children.Remove(shot);
                    _enemyShots.RemoveAt(i);
                }
            }
        }

        private bool IsCollision(Image a, Image b)
        {
            double aL = Canvas.GetLeft(a);
            double aT = Canvas.GetTop(a);
            double aR = aL + a.Width;
            double aB = aT + a.Height;

            double bL = Canvas.GetLeft(b);
            double bT = Canvas.GetTop(b);
            double bR = bL + b.Width;
            double bB = bT + b.Height;

            return aR > bL && aL < bR && aB > bT && aT < bB;
        }

        public void ClearAllShots()
        {
            foreach (var pb in _playerBullets)
                _gameCanvas.Children.Remove(pb);
            _playerBullets.Clear();

            foreach (var es in _enemyShots)
                _gameCanvas.Children.Remove(es);
            _enemyShots.Clear();
        }

        public void Restart()
        {
            ClearAllShots();
            _alienShotAnimationTimer = 0;
            _currentAlienShotFrame = 0;
        }
    }
}
