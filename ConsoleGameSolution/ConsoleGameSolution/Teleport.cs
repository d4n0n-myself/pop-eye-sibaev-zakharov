using System;

namespace ConsoleGameSolution
{
    public class Teleport : GameObject
    {
        public void PlaceTeleport(bool[,] walls, DestinationPoint point)
        {
            var random = new Random();
            var x = 0;
            var y = 0;

            while (walls[x, y] || (point.X == this.X && point.Y == this.Y))
            {
                x = random.Next(Field.XLimit) + 1;
                y = random.Next(Field.YLimit) + 1;
            }

            X = x;
            Y = y;

            WriteSymbol(X, Y, 'O');
        }
    }
}
