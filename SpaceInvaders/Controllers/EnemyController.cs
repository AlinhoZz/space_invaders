using Avalonia.Controls;
using Avalonia.Media.Imaging;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceInvaders.Controllers
{
    public class EnemyController
    {
        private readonly Canvas _gameCanvas;
        private readonly SoundManager _soundManager;
        private readonly ScoreController _scoreController;

        public List<Image> Enemies { get; private set; } = new();

        private Dictionary<string, Bitmap[]> _enemyAnimations = new();
        private Dictionary<string, int> _enemyScores = new()
        {
            { "Monster1", 40 },
            { "Monster2", 20 },
            { "Monster3", 10 }
        };

        private double _enemySpeedX = 0.3;
        private int _movementDirection = 1;
        private double _animationTimer = 0;
        private int _currentAnimationFrame = 0;

        private string[] _alienMovementSounds = new[]
        {
            "fastinvader1.wav",
            "fastinvader2.wav",
            "fastinvader3.wav",
            "fastinvader4.wav"
        };
        private int _alienMovementSoundIndex = 0;

        private Bitmap _explosionSprite;

        public EnemyController(Canvas gameCanvas,
                               SoundManager soundManager,
                               ScoreController scoreController)
        {
            _gameCanvas = gameCanvas;
            _soundManager = soundManager;
            _scoreController = scoreController;

            LoadEnemyAnimations();
            CreateEnemies();

            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            _explosionSprite = new Bitmap(Path.Combine(spritesPath, "MonsterExploding.png"));
        }

        private void LoadEnemyAnimations()
        {
            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            _enemyAnimations["Monster1"] = new[]
            {
                new Bitmap(Path.Combine(spritesPath, "Monster1-1.png")),
                new Bitmap(Path.Combine(spritesPath, "Monster1-2.png"))
            };
            _enemyAnimations["Monster2"] = new[]
            {
                new Bitmap(Path.Combine(spritesPath, "Monster2-1.png")),
                new Bitmap(Path.Combine(spritesPath, "Monster2-2.png"))
            };
            _enemyAnimations["Monster3"] = new[]
            {
                new Bitmap(Path.Combine(spritesPath, "Monster3-1.png")),
                new Bitmap(Path.Combine(spritesPath, "Monster3-2.png"))
            };
        }

        public void CreateEnemies()
        {
            ClearEnemies();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string monsterType;
                    if (j < 1) monsterType = "Monster1";
                    else if (j < 3) monsterType = "Monster2";
                    else monsterType = "Monster3";

                    var enemy = new Image
                    {
                        Width = 30,
                        Height = 30,
                        Tag = monsterType,
                        Source = _enemyAnimations[monsterType][0]
                    };
                    Canvas.SetLeft(enemy, 100 + (i * 50));
                    Canvas.SetTop(enemy, 50 + (j * 50));
                    _gameCanvas.Children.Add(enemy);
                    Enemies.Add(enemy);
                }
            }
        }

        public void ClearEnemies()
        {
            foreach (var e in Enemies)
                _gameCanvas.Children.Remove(e);
            Enemies.Clear();
        }

        public void Update()
        {
            _animationTimer += 16;
            if (_animationTimer >= 350)
            {
                _animationTimer = 0;

                var alienSound = _alienMovementSounds[_alienMovementSoundIndex];
                _soundManager.PlaySound(alienSound);
                _alienMovementSoundIndex = (_alienMovementSoundIndex + 1) % _alienMovementSounds.Length;

                _currentAnimationFrame = 1 - _currentAnimationFrame;
                foreach (var enemy in Enemies)
                {
                    if (enemy.Tag is string t && _enemyAnimations.ContainsKey(t))
                        enemy.Source = _enemyAnimations[t][_currentAnimationFrame];
                }
            }

            MoveEnemies();

            if (!_scoreController.AlreadyGameOverOrWin)
            {
                CheckIfEnemiesReachedBottom();
            }
        }

        private void MoveEnemies()
        {
            bool changeDirection = false;
            foreach (var enemy in Enemies)
            {
                double x = Canvas.GetLeft(enemy);
                if (x <= 0 || x >= 800 - enemy.Width)
                {
                    changeDirection = true;
                    break;
                }
            }

            if (changeDirection)
            {
                foreach (var enemy in Enemies)
                {
                    double y = Canvas.GetTop(enemy);
                    Canvas.SetTop(enemy, y + 5);
                }
                _movementDirection *= -1;
            }

            foreach (var enemy in Enemies)
            {
                double x = Canvas.GetLeft(enemy);
                Canvas.SetLeft(enemy, x + (_movementDirection * _enemySpeedX));
            }
        }
        
        private void CheckIfEnemiesReachedBottom()
        {
            const double bottomLimit = 520.0; 

            foreach (var enemy in Enemies)
            {
                double enemyY = Canvas.GetTop(enemy);
                if (enemyY >= bottomLimit)
                {
                    _scoreController.GameOver();
                    break;
                }
            }
        }

        public async Task KillEnemy(Image enemy)
        {
            Enemies.Remove(enemy);

            if (enemy.Tag is string eType && _enemyScores.ContainsKey(eType))
            {
                _scoreController.AddScore(_enemyScores[eType]);
                _scoreController.IncrementAliensKilled();
            }

            _soundManager.PlaySound("invaderkilled.wav");
            enemy.Source = _explosionSprite;

            await Task.Delay(200);
            _gameCanvas.Children.Remove(enemy);
        }

        public void IncreaseSpeed(double amount)
        {
            _enemySpeedX += amount;
        }

        public void Restart()
        {
            ClearEnemies();
            CreateEnemies();

            _enemySpeedX = 0.3;
            _movementDirection = 1;
            _animationTimer = 0;
            _currentAnimationFrame = 0;
            _alienMovementSoundIndex = 0;
        }
    }
}
