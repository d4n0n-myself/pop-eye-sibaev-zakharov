using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace ConsoleGameSolution
{
    /// <summary>
    /// Made by Danel Sibaev 11-706 and Zakharov Kirill 11-707
    /// </summary>

    class Program
    {
        private static bool stopGame = false;
        public static List<GameObject> Objects = new List<GameObject>();
        private static int score = 0;
        private static int livesCount = 3;
        
        public static int Level(int lvlNumber, char playerSymbol)
        {
            PrepareConsole();
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            var field = new Field();
            string fileName = Environment.CurrentDirectory.ToString() + "\\..\\..\\Levels\\Level" + lvlNumber + ".txt";
            bool[,] map = MapCreator.CreateMap(fileName);
            DrawWalls(map);
            GameObject.WriteLevelNumber(0, Field.YLimit + 3, "Level " + lvlNumber, ConsoleColor.Cyan);
            UpdateScore(score);

            #region<lvlEntities>
            var objects = MapCreator.GameObjects(fileName);

            var destinationPoint = new DestinationPoint();
            destinationPoint.Create();
            GameObject.WriteSymbol(destinationPoint.X, destinationPoint.Y, 'E', ConsoleColor.Green);

            var countOfButtons = objects["B"];
            var buttons = new Button().CreateButtons(countOfButtons);
            
            var rollingStones = new RollingStone().Create(map, objects["R"]);

            var ghosts = new Ghost().CreateGhosts(objects["G"]);
            
            var hearts = new Heart().CreateHearts(objects["H"]);
            
            var coins = new Coin().CreateCoins(objects["C"]);
            
            var demons = new Demon().CreateDemons(objects["D"]);
            
            var gate = new GameObject { X = Field.XLimit - 1, Y = 1 };
            GameObject.WriteSymbol(gate.X, gate.Y, '[', ConsoleColor.Red);
            map[gate.X, gate.Y] = true;
            #endregion

            Player player = new Player { X = 1, Y = Field.YLimit - 1, LivesCount = livesCount, Symbol = playerSymbol, color = ConsoleColor.Yellow };
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
                        stopGame = gameOver = true;
                        break;
                    }

                    player.Move(playerSymbol, map, keyPressed);
                }

                if (player.LivesCount <= 0)
                {
                    stopGame = death = gameOver = true;
                    break;
                }

                for (int i = 0; i < Math.Max(buttons.Count, Math.Max(hearts.Count, coins.Count)); i++)
                {
                    if (i < buttons.Count) GameObject.UpdateObject(buttons[i]);

                    if (i < hearts.Count) GameObject.UpdateObject(hearts[i]);

                    if (i < coins.Count) GameObject.UpdateObject(coins[i]);
                }

                for (int i = 0; i < ghosts.Count; i++)
                    ghosts[i].Move();
                for (int i = 0; i < rollingStones.Count; i++)
                    rollingStones[i].Move(map);

                GameObject.UpdateObject(player);

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

                for (int i=0; i< rollingStones.Count; i++)
                    if (player.X == rollingStones[i].X && player.Y == rollingStones[i].Y)
                    {
                        player.LivesCount--;
                        player.UpdateLivesCount();
                    }

                if (countOfButtons == 0)
                {
                    GameObject.WriteSymbol(gate.X, gate.Y, '[', ConsoleColor.Green);
                    map[gate.X, gate.Y] = false;
                }

                for (int i = 0; i < coins.Count; i++)
                    if (player.X == coins[i].X && player.Y == coins[i].Y)
                    {
                        coins.Remove(coins[i]);
                        score += 10;
                        UpdateScore(score);
                    }
                for (int i = 0; i < demons.Count; i++)
                {
                    demons[i].Move(player);                }
                for (int i = 0; i < demons.Count; i++)
                {
                    if (player.X == demons[i].X && player.Y == demons[i].Y)
                    {
                        player.LivesCount--;
                        player.UpdateLivesCount();
                    }
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
            else if (stopGame)
                Console.WriteLine("Goodbye.");
            else
            {
                Console.WriteLine("Level completed.");
                score += 1000;
            }

            Thread.Sleep(1000);
            return score;
        }

        #region <Systematic>
        public static void UpdateScore(int score)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, Field.YLimit + 2);
            Console.Write("Score:{0}", score);
        }

        public static void DrawWalls(bool[,] walls)
        {
            for (int i = 0; i < walls.GetLength(1); i++)
                for (int j = 0; j < walls.GetLength(0); j++)
                    if (walls[j, i])
                        GameObject.WriteSymbol(j, i, '█', ConsoleColor.DarkRed);
        }

        public static void PrepareConsole()
        {
            Console.Clear();
            Console.BufferWidth = Console.WindowWidth = 35;
            Console.BufferHeight = Console.WindowHeight = 27;
            Console.CursorVisible = false;
        }

        public static void DrawInterface()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BufferWidth = Console.WindowWidth = 60;
            Console.BufferHeight = Console.WindowHeight = 20;
            Console.CursorVisible = false;

            for (int x = 0; x < Console.WindowWidth - 1; x++)
                for (int y = 0; y < Console.WindowHeight; y++)
                    if (x < 2 || y < 2 || x > Console.WindowWidth - 3 || y > Console.WindowHeight - 3)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write('█');
                    }
        }

        public static void ShowFinalScore(int score)
        {
            DrawInterface();
            Console.SetCursorPosition(9, 7);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Your score:");
            Console.SetCursorPosition(9, 10);
            Console.WriteLine(score);
            Thread.Sleep(2000);
        }

        public static bool CheckAvailabilityToMoveToNextLevel(bool stopGame, int score)
        {
            if (stopGame)
            {
                DrawInterface();
                ShowFinalScore(score);
                return false;
            }
            return true;
        }
        #endregion

        public static void Main()
        {
            DrawInterface();
            Console.ForegroundColor = ConsoleColor.Red;
            Char playerSymbol = ' ';

            GameObject.Write(3, 3, "Rules:");
            GameObject.Write(3, 4, "Reach the exit, unlock it by stepping on \"■\".");
            GameObject.Write(3, 5, "Collect \"$\" on the way, it will increase your score.");
            GameObject.Write(3, 6, "Avoid ghosts \"Y\" and other mobs \"X\",\"o\".");
            GameObject.Write(3, 7, "\"H\" heart will increase your life count.");
            GameObject.Write(3, Console.BufferHeight - 5, "Choose your chip from following:");
            GameObject.Write(3, Console.BufferHeight - 4, "1 - @, 2 - є, 3 - &, 4 - %");
            Console.SetCursorPosition(3, Console.BufferHeight - 3);

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
                case "4":
                    playerSymbol = '%';
                    break;
                default:
                    Console.SetCursorPosition(9, 10);
                    Console.Write("Chip is not availible");
                    Thread.Sleep(1000);
                    return;
            }

            int gameScore = 0;

            for (int i = 1; i < 6; i++)
            {
                gameScore += Level(i, playerSymbol);
                if (!CheckAvailabilityToMoveToNextLevel(stopGame, gameScore))
                    return;
            }

            DrawInterface();
            ShowFinalScore(gameScore);

        }
    }
}