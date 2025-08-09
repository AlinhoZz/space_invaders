namespace SpaceInvaders.Models
{
    public class Player
    {
        public int Lives { get; set; } = 3;
        public double ShipX { get; set; }
        public double ShipWidth { get; set; } = 40;
        public double MoveSpeed { get; set; } = 6.0;
        public int MoveDirection { get; set; } = 0;
    }
}