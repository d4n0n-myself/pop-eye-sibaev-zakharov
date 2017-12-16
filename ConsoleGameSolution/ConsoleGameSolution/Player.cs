using System;
using System.Threading;

namespace ConsoleGameSolution
{
    public class Player : GameObject
    {
        public int LivesCount;
        public int Score;
        public bool Fallen = false;

        public void UpdateScore()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, Field.YLimit + 4);
            Console.Write("Score:{0}", Score);
        }

        public void UpdateLivesCount()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(0, Field.YLimit + 3);
            Console.Write("LivesCount:{0}", LivesCount);
        }

        public void ShowCoordinatesStatistics(char playerSymbol)
        {
            WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
            Console.SetCursorPosition(Field.XLimit - 10, Console.WindowHeight - 1);
            Console.Write("X: {0}  Y: {1,2}", X, Y);
        }

        private int CheckPointUnderPlayer(char playerSymbol, bool[,] walls, int X, int Y)
        {

            if (!walls[X, Y + 1])
            {
                Thread.Sleep(100);
                WriteSymbol(X, Y, ' ');
                Y++;
                WriteSymbol(X, Y, playerSymbol, ConsoleColor.Magenta);
                Thread.Sleep(100);
                WriteSymbol(X, Y, ' ');
                Y++;
                WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
                Fallen = true;
            }
            else Fallen = false;


            return Y;
        }

        public void Move(char playerSymbol, bool[,] walls, ConsoleKey direction)
        {
            WriteSymbol(X, Y, ' ');
            switch (direction)
            {
                case ConsoleKey.A:
                    if (X > 1 && !walls[X - 1, Y])
                    {
                        X--;
                        WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
                        Y = CheckPointUnderPlayer(playerSymbol, walls, X, Y);
                        while (Fallen)
                            Y = CheckPointUnderPlayer(playerSymbol, walls, X, Y);
                    }
                    break;
                case ConsoleKey.D:
                    if (X < Field.XLimit && !walls[X + 1, Y])
                    {
                        X++;
                        WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
                        Y = CheckPointUnderPlayer(playerSymbol, walls, X, Y);
                        while (Fallen)
                            Y = CheckPointUnderPlayer(playerSymbol, walls, X, Y);
                    }
                    break;
                case ConsoleKey.W:
                    if (Y > 1 && !walls[X, Y - 1])
                        Y--;
                    break;
                case ConsoleKey.S:
                    if (Y < Field.YLimit && !walls[X, Y + 1])
                        Y++;
                    break;
            }

            ShowCoordinatesStatistics(playerSymbol);
        }
    }

}
