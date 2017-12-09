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
        public static int playerScore;

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
            public void CreateCoordinatesStatistics(char playerSymbol)
            {
                Object.WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);
                Console.Write(X);
                Console.Write(' ');
                Console.Write(Y);
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
                }

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
                        }
                        break;
                    case ConsoleKey.D:
                        if (X < Field.XLimit && !walls[X + 1, Y])
                        {
                            X++;
                            WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
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

                WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);

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
                var countOfGhosts = random.Next(5, Field.YLimit / 2 + 1);

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

        public static bool[,] DrawWalls()
        {
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

            return gameWalls;
        }

        public static int Level1(char playerSymbol) 
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            // Создаём игровые объекты
            var field = new Field();

            ////d4n0n - Создать стены(пример обводки)
            var gameWalls = DrawWalls();

            //d4n0n - player create
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.CreateCoordinatesStatistics(playerSymbol);

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
                    player.Move(playerSymbol, gameWalls, keyPressed);
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
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 100);
                Thread.Sleep(sleepTime);
            }

            Console.Clear();

            if (death)
                Console.WriteLine("You've lost.");
            else score = 1000;

            Thread.Sleep(1000);
            return score;
        }

        public static int Level2(char playerSymbol)
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();
            
            var field = new Field();
            var gameWalls = DrawWalls();
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.CreateCoordinatesStatistics(playerSymbol);

            DestinationPoint destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

            bool death = false;
            bool gameOver = false;
            while (!gameOver)
            {
                stopwatch.Start();
                
                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape) break;
                    
                    player.Move(playerSymbol, gameWalls, keyPressed);
                }
                
                //d4n0n - подготовить новых мобов и вставить сюда физику их движения
                //или использовать Ghost

                //d4n0n - также добавить проверку на смерть игрока от этих врагов

                //Непонятно ? Смотри пример в Level1, он дописан.

                if (player.X == destinationPoint.X && player.Y == destinationPoint.Y)
                    gameOver = true;

                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 100);
                Thread.Sleep(sleepTime);
            }
            Console.Clear();

            if (death)
                Console.WriteLine("You've lost.");
            else score = 1000;

            Thread.Sleep(1000);
            return score;
        }

        public static void PrepareConsole()
        {
            Console.Clear();
            Console.BufferWidth = Console.WindowWidth = 36;
            Console.BufferHeight = Console.WindowHeight = 26;
            Console.CursorVisible = false;
        }

        public static void DrawInterface()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BufferWidth = Console.WindowWidth = 60;
            Console.BufferHeight = Console.WindowHeight = 20;
            Console.CursorVisible = false;

            for (int x = 0; x < Console.WindowWidth - 1; x++)
                for (int y = 0; y < Console.WindowHeight; y++)
                    if (x < 8 || y < 5 || x > Console.WindowWidth - 10 || y > Console.WindowHeight - 5)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write('#');
                    }
        }

        public static void ShowFinalScore()
        {
            DrawInterface();
            Console.SetCursorPosition(9, 7);
            Console.Write("Your score:");
            Console.SetCursorPosition(9, 10);
            Console.WriteLine(playerScore);
            Thread.Sleep(2000);
        }

        public static void Main()
        {
            DrawInterface();

            Console.SetCursorPosition(9, 6);
            Console.Write("Welcome! Choose your chip.");
            Console.WriteLine();
            Console.SetCursorPosition(9, 8);
            Console.Write("1 - @, 2 - e, 3 - &");
            Char playerSymbol = ' ';
            Console.SetCursorPosition(9, 10);

            switch (Console.ReadLine())
            {
                case "1":
                    playerSymbol = '@';
                    break;
                case "2":
                    playerSymbol = 'e';
                    break;
                case "3":
                    playerSymbol = '&';
                    break;
                default:
                    Console.SetCursorPosition(9, 10);
                    Console.Write("Chip is not availible");
                    Thread.Sleep(1000);
                    return;
            }

            playerScore += Level1(playerSymbol);
            
            if (playerScore < 1000)
            {
                ShowFinalScore();
                return;
            }

            playerScore += Level2(playerSymbol);

            DrawInterface();
            ShowFinalScore();

        }
    }
}