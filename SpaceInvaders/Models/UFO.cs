using Avalonia.Controls;

namespace SpaceInvaders.Models
{
    public class UFO
    {
        public bool IsHit { get; set; }
        public double Speed { get; set; } = 3.0;
        public bool MovingLeft { get; set; }
        public Image ImageControl { get; set; }
    }
}