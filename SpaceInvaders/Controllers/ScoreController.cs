using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceInvaders.Controllers
{
    public class ScoreController
    {
        private readonly GameController _gameController;
        private readonly TextBlock _scoreText;

        private int _score = 0;
        private static int _bestScore = 0;
        private int _aliensKilled = 0;
        private bool _extraLifeGiven = false;
        
        private bool _alreadyGameOverOrWin = false;
        public bool AlreadyGameOverOrWin => _alreadyGameOverOrWin;

        private DateTime _startTime;
        
        public event Action? OnRestartRequested;

        private List<TextBlock> _endGameTexts = new();

        public ScoreController(GameController gameController, TextBlock scoreText)
        {
            _gameController = gameController;
            _scoreText = scoreText;
            Restart();
        }

        public void Restart()
        {
            _score = 0;
            _aliensKilled = 0;
            _extraLifeGiven = false;
            _alreadyGameOverOrWin = false;

            _scoreText.Text = "0";
            _startTime = DateTime.Now;

            ClearEndGameUI();
        }

        public void AddScore(int val)
        {
            _score += val;
            if (_score > _bestScore)
                _bestScore = _score;

            _scoreText.Text = _score.ToString();
        }

        public void IncrementAliensKilled()
        {
            _aliensKilled++;
        }

        public void CheckGameStatus(PlayerController player, EnemyController enemy)
        {
            if (_alreadyGameOverOrWin) return;
            
            if (_score >= 10000)
            {
                _alreadyGameOverOrWin = true;
                ShowGameWin();
                return;
            }

            if (!_extraLifeGiven && _score >= 5000)
            {
                player.AddLife(1);
                _extraLifeGiven = true;
            }
        }

        public void GameOver()
        {
            if (_alreadyGameOverOrWin) return;
            _alreadyGameOverOrWin = true;
            ShowGameOver();
        }

        private void ShowGameOver()
        {
            _gameController.PauseAndRemoveAll();

            var canvas = _scoreText.Parent as Canvas;
            if (canvas == null) return;

            TimeSpan elapsed = DateTime.Now - _startTime;
            string recordSuffix = (_score >= _bestScore) ? " (novo recorde)" : "";

            var title = CreateTextBlock("GAME OVER", 60, Colors.LimeGreen);
            double currentY = 150;
            Canvas.SetTop(title, currentY);
            canvas.Children.Add(title);
            _endGameTexts.Add(title);
            CenterTextBlock(title, canvas);

            currentY += 70;
            var lines = new[]
            {
                $"Tempo: {elapsed:mm\\:ss}",
                $"Score: {_score}{recordSuffix}",
                $"Aliens derrotados: {_aliensKilled}"
            };
            foreach (var line in lines)
            {
                var tb = CreateTextBlock(line, 20, Colors.LimeGreen);
                Canvas.SetTop(tb, currentY);
                canvas.Children.Add(tb);
                _endGameTexts.Add(tb);
                CenterTextBlock(tb, canvas);
                currentY += 30;
            }

            var replay = CreateTextBlock("Jogar novamente?", 20, Colors.White);
            Canvas.SetTop(replay, currentY + 10);
            canvas.Children.Add(replay);
            _endGameTexts.Add(replay);
            CenterTextBlock(replay, canvas);

            replay.PointerPressed += (s, e) =>
            {
                ClearEndGameUI();
                OnRestartRequested?.Invoke();
            };
        }

        private void ShowGameWin()
        {
            _gameController.PauseAndRemoveAll();

            var canvas = _scoreText.Parent as Canvas;
            if (canvas == null) return;

            TimeSpan elapsed = DateTime.Now - _startTime;
            string recordSuffix = (_score >= _bestScore) ? " (novo recorde)" : "";

            var title = CreateTextBlock("YOU WIN!", 60, Colors.LimeGreen);
            double currentY = 150;
            Canvas.SetTop(title, currentY);
            canvas.Children.Add(title);
            _endGameTexts.Add(title);
            CenterTextBlock(title, canvas);

            currentY += 70;
            var lines = new[]
            {
                $"Tempo: {elapsed:mm\\:ss}",
                $"Score: {_score}{recordSuffix}",
                $"Aliens derrotados: {_aliensKilled}"
            };
            foreach (var line in lines)
            {
                var tb = CreateTextBlock(line, 20, Colors.LimeGreen);
                Canvas.SetTop(tb, currentY);
                canvas.Children.Add(tb);
                _endGameTexts.Add(tb);
                CenterTextBlock(tb, canvas);
                currentY += 30;
            }

            var replay = CreateTextBlock("Jogar novamente?", 20, Colors.White);
            Canvas.SetTop(replay, currentY + 10);
            canvas.Children.Add(replay);
            _endGameTexts.Add(replay);
            CenterTextBlock(replay, canvas);

            replay.PointerPressed += (s, e) =>
            {
                ClearEndGameUI();
                OnRestartRequested?.Invoke();
            };
        }

        public void ShowFloatingText(string text, double x, double y)
        {
            var canvas = _scoreText.Parent as Canvas;
            if (canvas == null) return;

            var txt = new TextBlock
            {
                Text = text,
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetLeft(txt, x);
            Canvas.SetTop(txt, y);
            canvas.Children.Add(txt);

            _ = RemoveTextAfter(txt, canvas, 1000);
        }

        private async Task RemoveTextAfter(TextBlock tb, Canvas canvas, int ms)
        {
            await Task.Delay(ms);
            if (canvas.Children.Contains(tb))
                canvas.Children.Remove(tb);
        }

        public void HideScore()
        {
            _scoreText.Text = "";
        }

        private TextBlock CreateTextBlock(string text, int fontSize, Color color)
        {
            return new TextBlock
            {
                FontFamily = _scoreText.FontFamily,
                FontSize = fontSize,
                Foreground = new SolidColorBrush(color),
                Text = text
            };
        }

        private void CenterTextBlock(TextBlock tb, Canvas canvas)
        {
            tb.Measure(Size.Infinity);
            tb.Arrange(new Rect(tb.DesiredSize));
            double left = (800 - tb.DesiredSize.Width) / 2;
            Canvas.SetLeft(tb, left);
        }

        private void ClearEndGameUI()
        {
            var canvas = _scoreText.Parent as Canvas;
            if (canvas == null) return;

            foreach (var t in _endGameTexts)
                canvas.Children.Remove(t);
            _endGameTexts.Clear();
        }
    }
}
