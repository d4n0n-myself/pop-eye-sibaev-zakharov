using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleGameSolution
{
    /// <summary>
    /// Made by Danel Sibaev 11-706 and Zakharov Kirill 11-707
    /// </summary>

    class Program
    {
        public class Field
        {
            public static int XLimit;
            public static int YLimit;

            public Field()
                : this(Console.WindowWidth - 1, Console.WindowHeight - 1)
            { }

            public Field(int xLimit, int yLimit)
            {
                XLimit = xLimit - 2;
                YLimit = yLimit - 2;
            }
        }

        public class Object
        {
            public int X { get; set; }
            public int Y { get; set; }

            public static void WriteSymbol(int x, int y, Char symbol)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(symbol);
            }

            public static void WriteSymbol(int x, int y, Char symbol, ConsoleColor color)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(symbol);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public class Player : Object
        {
            public void CreateCoordinatesStatistics()
            {
                Object.WriteSymbol(X, Y, '@', ConsoleColor.Yellow);
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);
                Console.Write(X);
                Console.Write(' ');
                Console.Write(Y);
            }

            private int CheckPointUnderPlayer(bool[,] walls, int X, int Y)
            {
                if (!walls[X, Y + 1])
                {
                    Thread.Sleep(100);
                    WriteSymbol(X, Y, ' ');
                    Y++;
                    WriteSymbol(X, Y, '@', ConsoleColor.Magenta);
                    Thread.Sleep(100);
                    WriteSymbol(X, Y, ' ');
                    Y++;
                    WriteSymbol(X, Y, '@', ConsoleColor.Yellow);
                }

                return Y;
            }

            public void Move(bool[,] walls, ConsoleKey direction)
            {
                WriteSymbol(X, Y, ' ');
                switch (direction)
                {
                    case ConsoleKey.A:
                        if (X > 1 && !walls[X - 1, Y])
                        {
                            X--;
                            WriteSymbol(X, Y, '@', ConsoleColor.Yellow);
                            Y = CheckPointUnderPlayer(walls, X, Y);
                        }
                        break;
                    case ConsoleKey.D:
                        if (X < Field.XLimit && !walls[X + 1, Y])
                        {
                            X++;
                            WriteSymbol(X, Y, '@', ConsoleColor.Yellow);
                            Y = CheckPointUnderPlayer(walls, X, Y);
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
                
                WriteSymbol(X, Y, '@', ConsoleColor.Yellow);

                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);
                Console.Write(X);
                Console.Write(' ');
                Console.Write(Y);
            }
        }

        public class DestinationPoint : Object
        {
            public DestinationPoint Create()
            {
                var random = new Random();
                X = random.Next(Field.XLimit) + 1;
                Y = 1;
                return this;
            }
        }

        public class Ghost : Object
        {
            public bool DirectedToRightSide;

            public List<Ghost> CreateGhosts()
            {
                var ghosts = new List<Ghost>();
                var random = new Random();
                var countOfGhosts = random.Next(3, Field.YLimit / 2 + 1);

                for (int i = 0; i < countOfGhosts; i++)
                {
                    var xPos = random.Next(1, Field.XLimit);
                    var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;
                    var randomDirection = random.Next(2);

                    foreach (var ghost in ghosts)
                        if (ghost.Y == yPos)
                            yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

                    ghosts.Add(new Ghost { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                    WriteSymbol(ghosts[i].X, ghosts[i].Y, 'X', ConsoleColor.Red);
                }

                return ghosts;
            }

            public void Move()
            {
                WriteSymbol(X, Y, ' ', ConsoleColor.White);

                if (DirectedToRightSide && X < Field.XLimit + 2)
                    X++;
                if (X == Field.XLimit + 1) DirectedToRightSide = false;
                if (DirectedToRightSide == false && X > 0)
                    X--;
                if (X == 1) DirectedToRightSide = true;

                WriteSymbol(X, Y, 'X', ConsoleColor.Red);
            }
        }

        public static void Main()
        {
            Console.BufferWidth = Console.WindowWidth = 60;
            Console.BufferHeight = Console.WindowHeight = 20;
            Console.CursorVisible = false;
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            // Создаём игровые объекты
            var field = new Field();

            ////d4n0n - Создать стены(пример обводки)
            bool[,] gameWalls = new bool[Field.XLimit + 2, Field.YLimit + 2];

            for (int i = 0; i < Field.XLimit + 2; i++)
                for (int j = 0; j < Field.YLimit + 2; j++)
                    if (j % 2 == 0 || i == 0 || j == 0 || i == Field.XLimit + 1 || j == Field.YLimit + 1)
                        gameWalls[i, j] = true;

            var random = new Random();
            int temp = random.Next(1, Field.XLimit);

            for (int j = 1; j < Field.YLimit; j++)
            {
                gameWalls[temp, j] = false;
                temp = random.Next(1, Field.XLimit + 1);
                gameWalls[temp, j] = false;
                temp = random.Next(1, Field.XLimit + 1);
            }

            for (int i = 0; i < Field.XLimit + 2; i++)
                for (int j = 0; j < Field.YLimit + 2; j++)
                    if (gameWalls[i, j])
                        Object.WriteSymbol(i, j, '#', ConsoleColor.Gray);

            //d4n0n - player create
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.CreateCoordinatesStatistics();

            //d4n0n - создать ghost'ов
            var ghosts = new Ghost().CreateGhosts();

            //d4n0n - создание destination point
            DestinationPoint destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

            bool death = false;
            bool gameOver = false;
            //Игровой цикл
            while (!gameOver)
            {
                stopwatch.Start();
                
                // Обработка нажатий клавиатуры
                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape) break;

                    // Перемещение объектов по нажатию кнопок
                    player.Move(gameWalls, keyPressed);
                }

                // d4n0n - ДВИЖЕНИЕ ghosts
                for (int i = 0; i < ghosts.Count; i++)
                    ghosts[i].Move();

                //d4n0n - проверка на столкновение
                for (int i = 0; i < ghosts.Count; i++)
                    if (player.X == ghosts[i].X && player.Y == ghosts[i].Y)
                    {
                        gameOver = true;
                        death = true;
                    }

                //d4n0n -  проверка на достижение цели
                if (player.X == destinationPoint.X && player.Y == destinationPoint.Y)
                    gameOver = true;

                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 80);
                Thread.Sleep(sleepTime);
            }

            Console.Clear();

            if (death)
                Console.WriteLine("You've lost.");

            else Console.WriteLine("You win!");
            Console.WriteLine("Escape to exit.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            { }
        }
    }
}