using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ConsoleGameSolution
{
    /// <summary>
    /// Made by Danel Sibaev 11-706 and Zakharov Kirill 11-707
    /// </summary>
    /// <ThingsTODO>
    /// Перенос количества жизней на следующий уровень
    /// Проверка что на указанной позиции ничего нет
    /// </ThingsTODO>
    class Program
    {
        public static int playerScore;
        public static bool death;
        public static List<GameObject> Objects = new List<GameObject>();

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
                        ConsoleGameSolution.GameObject.WriteSymbol(i, j, '#', ConsoleColor.Gray);

            return gameWalls;
        }

        //d4n0n - preset for level
        //to make it generalized
        public static int Level(int lvlNumber, string fileName, char playerSymbol)
        {
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var map = MapCreator.CreateMap(fileName);
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level " + lvlNumber, ConsoleColor.Cyan);

            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

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
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }

                    player.Move(playerSymbol, map, keyPressed);
                }

                if (player.X == destinationPoint.X && player.Y == destinationPoint.Y)
                    gameOver = true;

                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 0);
                stopwatch.Reset();
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

        #region <Levels>
        public static int Level1(char playerSymbol)// Kiri; мой тест уровень, не удалять
        {
            #region <Default parametrs and creation>
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();
            var field = new Field();
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level 1", ConsoleColor.Cyan);
            var gameWalls = DrawWalls();
            var ghosts = new Ghost().CreateGhosts();

            var countOfButtons = 4;
            var buttons = new Button().CreateButtons(countOfButtons);
            for (int i = 0; i < buttons.Count; i++)
            {
                var button = new GameObject { X = buttons[i].X, Y = buttons[i].Y };
                GameObject.WriteSymbol(button.X, button.Y, button.Symbol, button.color);
            }

            var hearts = new Heart().CreateHearts();
            for (int i = 0; i < hearts.Count; i++)
            {
                var heart = new GameObject { X = hearts[i].X, Y = hearts[i].Y };
                GameObject.WriteSymbol(heart.X, heart.Y, heart.Symbol, heart.color);
            }

            var countOfCoins = 10;
            var coins = new Coin().CreateCoins(countOfCoins);
            for (int i = 0; i < coins.Count; i++)
            {
                var coin = new GameObject { X = coins[i].X, Y = coins[i].Y }; //Данэль,эта хрень не нужна аще
                GameObject.WriteSymbol(coin.X, coin.Y, coin.Symbol, coin.color);
            }

            var gate = new GameObject { X = Field.XLimit - 2, Y = 1 };
            GameObject.WriteSymbol(gate.X, gate.Y, '[', ConsoleColor.Red);
            gameWalls[gate.X, gate.Y] = true;

            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

            Player player = new Player { X = 1, Y = Field.YLimit , color=ConsoleColor.Yellow, Symbol=playerSymbol ,LivesCount=3};
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

            
            bool gameOver = false;
            #endregion

            while (!gameOver)
            {
                stopwatch.Start();

                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }

                    player.Move(playerSymbol, gameWalls, keyPressed);
                }
                if (player.LivesCount == 0)
                {
                    gameOver = true;
                    death = true;
                }

                for (int i = 0; i < buttons.Count; i++)
                {
                    GameObject.UpdateObject(buttons[i]);
                }
                GameObject.UpdateObject(player);

                for (int i = 0; i < hearts.Count; i++)
                {
                    GameObject.UpdateObject(hearts[i]);
                }

                for (int i = 0; i < coins.Count; i++)
                {
                    GameObject.UpdateObject(coins[i]);
                }

                for (int i = 0; i < ghosts.Count; i++)
                    ghosts[i].Move();


                for (int i = 0; i < ghosts.Count; i++)
                    if (player.X == ghosts[i].X && player.Y == ghosts[i].Y)
                    {
                        player.LivesCount--;
                        player.UpdateLivesCount();
                    }

                for (int i = 0; i < hearts.Count; i++)
                    if (player.X == hearts[i].X && player.Y == hearts[i].Y)
                    {
                        hearts.Remove(hearts[i]);
                        player.LivesCount++;
                        player.UpdateLivesCount();
                    }

                for (int i = 0; i < buttons.Count; i++)
                    if (player.X == buttons[i].X && player.Y == buttons[i].Y && buttons[i].IsStepped == false)
                    {
                        countOfButtons--;
                        buttons[i].IsStepped = true;
                        buttons[i].color = ConsoleColor.Green;
                    }
                if (countOfButtons == 0)
                {
                    GameObject.WriteSymbol(gate.X, gate.Y, '[', ConsoleColor.Green);
                    gameWalls[gate.X, gate.Y] = false;
                }

                for (int i = 0; i < coins.Count; i++)
                    if (player.X == coins[i].X && player.Y == coins[i].Y)
                    {
                        coins.Remove(coins[i]);
                        player.Score+=10;
                        //player.UpdateScore();
                    }


                if (player.X == destinationPoint.X && player.Y == destinationPoint.Y)
                    gameOver = true;

                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 0);
                stopwatch.Reset();
                Thread.Sleep(sleepTime);
            }

            Console.Clear();

            if (death)
                Console.WriteLine("You lose.");
            else
            {
                Console.WriteLine("Level completed.");
                //score = 1000;
            }


            Thread.Sleep(1000);
            return score;
        }

        public static int Level2(char playerSymbol)
        {
            #region <Default parametrs and creation>
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level 2", ConsoleColor.Blue);
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            var teleport = new Teleport();
            teleport.PlaceTeleport(gameWalls, destinationPoint);

            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

            //d4n0n - Create Falling Balls
            var balls = new Ball().Create(gameWalls);

            bool death = false;
            bool gameOver = false;
            #endregion

            while (!gameOver)
            {
                stopwatch.Start();

                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }

                    player.Move(playerSymbol, gameWalls, keyPressed);
                }

                //d4n0n - подготовить новых мобов и вставить сюда физику их движения
                //или использовать Ghost


                //d4n0n - также добавить проверку на смерть игрока от этих врагов

                //Непонятно ? Смотри пример в Level3, он дописан.

                if (player.X == teleport.X && player.Y == teleport.Y)
                {
                    GameObject.WriteSymbol(player.X, player.Y, ' ');
                    var random = new Random();
                    teleport.PlaceTeleport(gameWalls, destinationPoint);
                    player.X = teleport.X;
                    player.Y = teleport.Y;
                    GameObject.WriteSymbol(player.X, player.Y, playerSymbol, ConsoleColor.Blue);
                    GameObject.WriteSymbol(player.X, player.Y, ' ');
                    GameObject.WriteSymbol(player.X, player.Y, playerSymbol, ConsoleColor.Yellow);
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
                        GameObject.WriteSymbol(teleport.X, teleport.Y, ' ');
                        teleport.X = 0;
                        teleport.Y = 0;
                    }
                }

                GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

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
            #region <Default parametrs and creation>
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            // Создаём игровые объекты
            var field = new Field();
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level 3", ConsoleColor.Red);

            ////d4n0n - Создать стены(пример обводки)
            var gameWalls = DrawWalls();

            //d4n0n - player create
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

            //d4n0n - создать ghost'ов
            var ghosts = new Ghost().CreateGhosts();

            //d4n0n - создание destination point
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);

            bool death = false;
            bool gameOver = false;
            #endregion
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
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }
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

        public static int Level4(char playerSymbol)
        {
            #region <Default parametrs and creation>
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level 4", ConsoleColor.DarkYellow);
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

            bool death = false;
            bool gameOver = false;
            #endregion

            while (!gameOver)
            {
                stopwatch.Start();

                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }

                    player.Move(playerSymbol, gameWalls, keyPressed);
                }

                //d4n0n - подготовить новых мобов и вставить сюда физику их движения
                //или использовать Ghost
                //или Ball

                //d4n0n - также добавить проверку на смерть игрока от этих врагов

                //Непонятно ? Смотри пример в Level3, он дописан.

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
            #region <Default parametrs and creation>
            int score = 0;
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            GameObject.WriteLevelNumber(0, Field.YLimit + 2, "Level 5", ConsoleColor.Magenta);
            var gameWalls = DrawWalls();
            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, '%', ConsoleColor.Green);
            Player player = new Player { X = 1, Y = Field.YLimit };
            player.ShowCoordinatesStatistics(playerSymbol);
            player.UpdateLivesCount();

            bool death = false;
            bool gameOver = false;
            #endregion

            while (!gameOver)
            {
                stopwatch.Start();

                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape)
                    {
                        death = true;
                        break;
                    }

                    player.Move(playerSymbol, gameWalls, keyPressed);
                }

                //d4n0n - подготовить новых мобов и вставить сюда физику их движения
                //или использовать Ghost
                //или Ball

                //d4n0n - также добавить проверку на смерть игрока от этих врагов

                //Непонятно ? Смотри пример в Level3, он дописан.

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
        #endregion

        #region <Systematic>
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

        public static bool CheckAvailabilityToMoveToNextLevel(bool death)
        {
            if (death)
            {
                DrawInterface();
                ShowFinalScore();
                return false;
            }
            return true;
        }
        #endregion

        public static void Main()
        {
            DrawInterface();

            Console.SetCursorPosition(9, 6);
            Console.Write("Welcome! Choose your chip.");
            Console.WriteLine();
            Console.SetCursorPosition(9, 8);
            Console.Write("1 - @, 2 - є, 3 - &");
            Char playerSymbol = ' ';
            Console.SetCursorPosition(9, 10);

            switch (Console.ReadLine())
            {
                case "1":
                    playerSymbol = '@';
                    break;
                case "2":
                    playerSymbol = 'є';
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
            if (!CheckAvailabilityToMoveToNextLevel(death)) return;

            playerScore += Level2(playerSymbol);
            if (!CheckAvailabilityToMoveToNextLevel(death)) return;

            playerScore += Level3(playerSymbol);
            if (!CheckAvailabilityToMoveToNextLevel(death)) return;

            playerScore += Level4(playerSymbol);
            if (!CheckAvailabilityToMoveToNextLevel(death)) return;

            playerScore += Level5(playerSymbol);

            DrawInterface();
            ShowFinalScore();

        }
    }
}