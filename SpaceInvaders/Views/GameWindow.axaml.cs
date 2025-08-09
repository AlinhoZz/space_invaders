using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SpaceInvaders.Controllers;
using System;

namespace SpaceInvaders.Views
{
    public partial class GameWindow : Window
    {
        private Canvas _gameCanvas;
        private Image _playerShip;
        private TextBlock _scoreText;
        private Image _life1, _life2, _life3;

        private DispatcherTimer _gameLoopTimer;
        private DispatcherTimer _speedIncreaseTimer;

        private GameController _controller;

        public GameWindow()
        {
            InitializeComponent();
            
            _gameCanvas = this.FindControl<Canvas>("GameCanvas")!;
            _playerShip = this.FindControl<Image>("PlayerShip")!;
            _scoreText  = this.FindControl<TextBlock>("ScoreText")!;
            _life1      = this.FindControl<Image>("Life1")!;
            _life2      = this.FindControl<Image>("Life2")!;
            _life3      = this.FindControl<Image>("Life3")!;
            
            _controller = new GameController(
                _gameCanvas,
                _playerShip,
                _scoreText,
                _life1,
                _life2,
                _life3
            );
            
            _gameLoopTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _gameLoopTimer.Tick += (s, e) => _controller.OnGameLoop();
            _gameLoopTimer.Start();
            
            _speedIncreaseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(7) };
            _speedIncreaseTimer.Tick += (s, e) => _controller.IncreaseEnemySpeed();
            _speedIncreaseTimer.Start();

            Focusable = true;
            Focus();
            KeyDown += OnKeyDown;
            KeyUp   += OnKeyUp;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _controller.HandleKeyDown(e);
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _controller.HandleKeyUp(e);
        }
    }
}
