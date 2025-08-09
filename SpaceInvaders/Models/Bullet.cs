using Avalonia.Controls;

namespace SpaceInvaders.Models
{
    public class Bullet
    {
        public Image ImageControl { get; set; } = null!;  
        public double Width { get; set; }
        public double Height { get; set; }
    }
}