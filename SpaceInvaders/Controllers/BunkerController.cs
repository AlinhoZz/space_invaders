using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpaceInvaders.Controllers
{
    public class BunkerController
    {
        private readonly Canvas _gameCanvas;
        private Dictionary<Image, int> _bunkerDamageStage = new();
        private List<Bitmap> _bunkerStages = new();

        public BunkerController(Canvas gameCanvas)
        {
            _gameCanvas = gameCanvas;
            LoadBunkerStages();
            RegisterBunkers();
        }

        private void LoadBunkerStages()
        {
            var spritesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sprites");
            for (int i = 1; i <= 7; i++)
            {
                var file = Path.Combine(spritesPath, $"Wall{i}.png");
                if (File.Exists(file))
                    _bunkerStages.Add(new Bitmap(file));
            }
        }

        private void RegisterBunkers()
        {
            foreach (var child in _gameCanvas.Children)
            {
                if (child is Image img && img.Tag?.ToString() == "Bunker")
                {
                    _bunkerDamageStage[img] = 0;
                    if (_bunkerStages.Count > 0)
                        img.Source = _bunkerStages[0];
                }
            }
        }

        public void DamageBunker(Image bunker)
        {
            if (!_bunkerDamageStage.ContainsKey(bunker)) return;

            int stage = _bunkerDamageStage[bunker];
            stage++;
            if (stage < _bunkerStages.Count)
            {
                bunker.Source = _bunkerStages[stage];
                _bunkerDamageStage[bunker] = stage;
            }
            else
            {
                _gameCanvas.Children.Remove(bunker);
                _bunkerDamageStage.Remove(bunker);
            }
        }
        
        public void Restart()
        {
            ClearAllBunkers();
            
            var b1 = new Image { Tag="Bunker", Width=60, Height=40 };
            Canvas.SetLeft(b1,130); Canvas.SetTop(b1,480);
            _gameCanvas.Children.Add(b1);

            var b2 = new Image { Tag="Bunker", Width=60, Height=40 };
            Canvas.SetLeft(b2,300); Canvas.SetTop(b2,480);
            _gameCanvas.Children.Add(b2);

            var b3 = new Image { Tag="Bunker", Width=60, Height=40 };
            Canvas.SetLeft(b3,470); Canvas.SetTop(b3,480);
            _gameCanvas.Children.Add(b3);

            var b4 = new Image { Tag="Bunker", Width=60, Height=40 };
            Canvas.SetLeft(b4,640); Canvas.SetTop(b4,480);
            _gameCanvas.Children.Add(b4);

            RegisterBunkers();
        }

        public void ClearAllBunkers()
        {
            var bunkers = _gameCanvas.Children
                .OfType<Image>()
                .Where(x => x.Tag?.ToString() == "Bunker")
                .ToList();
            foreach (var b in bunkers)
                _gameCanvas.Children.Remove(b);

            _bunkerDamageStage.Clear();
        }
    }
}
