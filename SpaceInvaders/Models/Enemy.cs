using Avalonia.Controls;

namespace SpaceInvaders.Models
{
    public class Enemy
    {
        public string Type { get; set; } = null!;
        public double Width { get; set; }
        public double Height { get; set; }
        public Image ImageControl { get; set; } = null!;
    }
}