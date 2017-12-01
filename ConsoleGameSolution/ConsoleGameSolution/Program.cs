using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace ConsoleGameSolution
{
    /// <summary>
    /// Made by Danel Sibaev 11-706 and Kirill Zakharov 11-707
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.BufferWidth = Console.WindowWidth = 32;
            Console.BufferHeight = Console.WindowHeight = 22;
            Console.CursorVisible = false;
            // Установка параметра задержки смены кадров
            const int frameDelay = 100;
            
            bool gameOver = false;
            
            while (!gameOver)
            {
                var stopwatch = new Stopwatch();
                if (Console.KeyAvailable)
                {
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape) break;
                        
                    // Перемещение объектов по нажатию кнопок
                    ConsoleGame.Move(keyPressed);
                    
                    // Вычисление задержки для поддержания стабильного FPS
                    stopwatch.Stop();
                    int sleepTime = Math.Max(frameDelay - (int) stopwatch.Elapsed.TotalMilliseconds,0);
                    // Задержка
                    Thread.Sleep(sleepTime);
                }

            }
            Console.Clear();
        }
    }

    public class Ghost
    {
        
    }

    public class ConsoleGame
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }
        public static int XLimit;
        public static int YLimit;

        public void CollectLimits(int xLimit, int yLimit)
        {
            XLimit = xLimit;
            YLimit = yLimit;
        }
        
        public static void Move(ConsoleKey dir)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(' ');
            switch (dir)
            {
                    case ConsoleKey.W:
                        if (Y > 0) Y--;
                        break;
                    case ConsoleKey.A:
                        if (X > 0) X--;
                        break;
                    case ConsoleKey.S:
                        if (Y < XLimit) Y++;
                        break;
                    case ConsoleKey.D:
                        if (X < YLimit) X++;
                        break;
            }
            Console.SetCursorPosition(X,Y);
            Console.Write('@');
        }

    }
}