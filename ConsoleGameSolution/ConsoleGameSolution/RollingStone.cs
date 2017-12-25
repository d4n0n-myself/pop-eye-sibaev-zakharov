using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGameSolution
{
    public class RollingStone : GameObject
    {
        private bool DirectedToRightSide;

        public List<RollingStone> Create(bool[,] walls,int count)
        {
            var balls = new List<RollingStone>();
            var random = new Random();
            

            for (int i = 0; i < count; i++)
            {
                var xPos = random.Next(Field.XLimit) + 1;
                var yPos = random.Next(Field.YLimit / 4) * 2 + 1;
                var randomDirection = random.Next(2);

                foreach (var obj in Program.Objects)
                    if (obj.Y == yPos && obj.X == xPos)
                    {
                        xPos = random.Next(1, Field.XLimit);
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;
                    }

                balls.Add(new RollingStone { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                WriteSymbol(balls[i].X, balls[i].Y, 'Θ', ConsoleColor.Blue);
            }

            return balls;
        }

        public void Move(bool[,] walls)
        {
            WriteSymbol(X, Y, ' ');

            if (Y + 1 < Field.YLimit + 1 && !walls[X, Y + 1])
            {
                Y++;
                WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                return;
            }
            if (X + 1 < Field.XLimit && DirectedToRightSide)
            {
                X++;
                WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                return;
            }
            if (X - 1 > 1 && !DirectedToRightSide)
            {
                X--;
                WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                return;
            }
            if (Y == Field.YLimit - 1) Y = 1;
            if (X == 2) DirectedToRightSide = true;
            if (X == Field.XLimit - 1) DirectedToRightSide = false;
            Thread.Sleep(50);
        }
    }
}
