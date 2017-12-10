using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

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
            public void ShowCoordinatesStatistics(char playerSymbol)
            {
                Object.WriteSymbol(X, Y, playerSymbol, ConsoleColor.Yellow);
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight - 1);
                Console.Write("X: {0}  Y: {1}",X,Y);
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

                ShowCoordinatesStatistics(playerSymbol);
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

        public class Ball : Object
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
                var countOfBalls = random.Next(4,6);

                for (int i = 0; i < countOfBalls; i++)
                {
                    var xPos = random.Next(Field.XLimit) + 1;
                    var yPos = random.Next(Field.YLimit / 4) * 2 + 1;
                    var randomDirection = random.Next(2);

                    foreach (var ball in balls)
                        ball.CanTakePoint(yPos,walls);

                    balls.Add(new Ball { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                    WriteSymbol(balls[i].X, balls[i].Y, 'o', ConsoleColor.Blue);
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
                if (X == Field.XLimit) DirectedToRightSide = false;
                if (X - 1 > 0 && !DirectedToRightSide)
                {
                    X--;
                    WriteSymbol(X, Y, 'o', ConsoleColor.Blue);
                    return;
                }
                if (X == 1) DirectedToRightSide = true;
            }
        }

        public class Teleport : Object
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

                WriteSymbol(X,Y,'O');
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
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Level 1");

            ////d4n0n - Создать стены(пример обводки)
            var gameWalls = DrawWalls();

            //d4n0n - player create
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);

            //d4n0n - создать ghost'ов
            var ghosts = new Ghost().CreateGhosts();

            //d4n0n - создание destination point
            var destinationPoint = new DestinationPoint();
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
            else
            {
                Console.WriteLine("Level completed.");
                score = 1000;
            }

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
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Level 2");
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            var teleport = new Teleport();
            teleport.PlaceTeleport(gameWalls,destinationPoint);
            
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);

            //d4n0n - Create Falling Balls
            var balls = new Ball().Create(gameWalls);

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

                if (player.X == teleport.X && player.Y == teleport.Y)
                {
                    Object.WriteSymbol(player.X, player.Y, ' ');
                    var random = new Random();
                    teleport.PlaceTeleport(gameWalls,destinationPoint);
                    player.X = teleport.X;
                    player.Y = teleport.Y;
                    Object.WriteSymbol(player.X, player.Y, playerSymbol, ConsoleColor.Blue);
                    Object.WriteSymbol(player.X, player.Y, ' ');
                    Object.WriteSymbol(player.X, player.Y, playerSymbol, ConsoleColor.Yellow);
                    teleport.X = 0;
                    teleport.Y = 0;
                }

                foreach (var ball in balls)
                {
                    ball.Move(gameWalls);
                    if (player.X == ball.X && player.Y == ball.Y)
                    {
                        death = true;
                        gameOver = true;
                    }
                    if (ball.X == teleport.X && ball.Y == teleport.Y)
                    {
                        Object.WriteSymbol(teleport.X, teleport.Y, ' ');
                        teleport.X = 0;
                        teleport.Y = 0;
                    }
                }

                Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

                if (player.X == destinationPoint.X && player.Y == destinationPoint.Y)
                    gameOver = true;

                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 100);
                Thread.Sleep(sleepTime);
            }

            Console.Clear();

            if (death)
                Console.WriteLine("You've lost.");
            else
            {
                Console.WriteLine("Level completed.");
                score = 1000;
            }


            Thread.Sleep(1000);
            return score;
        }

        public static int Level3(char playerSymbol)
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Level 3");
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);

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
                //или Ball

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
            else
            {
                Console.WriteLine("Level completed.");
                score = 1000;
            }


            Thread.Sleep(1000);
            return score;
        }

        public static int Level4(char playerSymbol)
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Level 4");
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);

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
                //или Ball

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
            else
            {
                Console.WriteLine("Level completed.");
                score = 1000;
            }


            Thread.Sleep(1000);
            return score;
        }

        public static int Level5(char playerSymbol)
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Level 4");
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            Object.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);

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
                //или Ball

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
            else
            {
                Console.WriteLine("Level completed.");
                score = 1000;
            }


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
                    if (x < 3 || y < 3 || x > Console.WindowWidth - 5 || y > Console.WindowHeight - 4)
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

            if (playerScore < 2000)
            {
                ShowFinalScore();
                return;
            }

            playerScore += Level3(playerSymbol);

            if (playerScore < 3000)
            {
                ShowFinalScore();
                return;
            }

            playerScore += Level4(playerSymbol);

            if (playerScore < 4000)
            {
                ShowFinalScore();
                return;
            }

            playerScore += Level5(playerSymbol);

            DrawInterface();
            ShowFinalScore();

        }
    }
}