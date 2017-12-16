using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Ball : GameObject
    {
        public bool DirectedToRightSide;

        public void CanTakePoint(int yPos, bool[,] walls)
        {
            var random = new Random();

            while (Y == yPos || walls[X, Y])
                yPos = random.Next(Field.YLimit / 4) + Field.YLimit / 4;
        }

        public List<Ball> Create(bool[,] walls)
        {
            var balls = new List<Ball>();
            var random = new Random();
            var countOfBalls = random.Next(4, 6);

            for (int i = 0; i < countOfBalls; i++)
            {
                var xPos = random.Next(Field.XLimit) + 1;
                var yPos = random.Next(Field.YLimit / 4) * 2 + 1;
                var randomDirection = random.Next(2);

                foreach (var ball in balls)
                    ball.CanTakePoint(yPos, walls);

                balls.Add(new Ball { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
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
            if (X + 1 < Field.XLimit + 1 && DirectedToRightSide)
            {
                X++;
                WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                return;
            }
            if (X - 1 > 0 && !DirectedToRightSide)
            {
                X--;
                WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                return;
            }
            if (X == 1 && Y == Field.YLimit) Y = 1;
            if (X == 1) DirectedToRightSide = true;
            if (X == Field.XLimit) DirectedToRightSide = false;
        }
    }
}
