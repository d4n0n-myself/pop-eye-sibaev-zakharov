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
        public class At
        {
            public static int XLimit;
            public static int YLimit;
            
            public At()
                : this(Console.WindowWidth - 1, Console.WindowHeight - 2)
            { }

            public At(int xLimit, int yLimit)
            {
                XLimit = xLimit;
                YLimit = yLimit;
            }
        }

        public class Entity
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        
        public class Player : Entity
        {
            public void Create()
            {
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
            }
            
            public void Move(ConsoleKey direction)
            {
                // Стираем следы (все пиксели игрового объекта на поле)
                Console.SetCursorPosition(X, Y);
                Console.Write(' ');
                switch (direction)
                {
                    //для первого игрока
                    case ConsoleKey.A:
                        if (X > 1) X--;
                        break;
                    case ConsoleKey.D:
                        if (X < At.XLimit - 2) X++;
                        break;
                    case ConsoleKey.W:
                        if (Y > 1) Y--;
                        break;
                    case ConsoleKey.S:
                        if (Y < At.YLimit - 1)
                            Y++;
                        break;
                        
                    //для второго игрока
                    case ConsoleKey.UpArrow:
                        if (Y > 1) Y--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (Y < At.YLimit - 1) Y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (X > 1) X--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (X < At.XLimit - 2) X++;
                        break;
                }
                // Отрисовываем объект в новой позиции
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
            }
        }

        public class Ghost : Entity
        {
        }

        public class DestinationPoint : Entity
        {
        }

        public static List<Ghost> CreateGhosts()
        {
            var ghosts = new List<Ghost>();
            var random = new Random();
            //d4n0n - попытка обработать гостов
            
            for (int i=0;i<3;i++)
            {
                ghosts.Add(new Ghost { X = random.Next(1, 89), Y = random.Next(1, 29) });
                Console.SetCursorPosition(ghosts[i].X, ghosts[i].Y);
                Console.Write('2');
            }

            return ghosts;
        }

        static void Main()
        {
            // Инициализация параметров
            Console.BufferWidth = Console.WindowWidth = 91;
            Console.BufferHeight = Console.WindowHeight = 31;
            // Прячем курсор для красоты
            Console.CursorVisible = false;
            // Установка параметра задержки смены кадров
            const int frameDelay = 100;
            var stopwatch = new Stopwatch();

            //d4n0n - Создать стены(пример обводки)
            for(int i=0;i<30;i++)
                for (int j=0;j<90;j++)
                    if (i == 0 || j == 0 || i == 29 || j == 89)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write('#');
                    }

            //d4n0n - создать ghost'ов
            var ghosts = CreateGhosts();
            
            // Создаём игровые объекты
            var at = new At();

            
            //d4n0n - создание player ов
            Player[] players = new Player[2];
            for (int i = 0; i < 2; i++)
                players[i].Create();
            
            //d4n0n - создание destination point
            DestinationPoint point = new DestinationPoint();
            var random = new Random();
            point.X = random.Next(At.XLimit) + 1;
            point.Y = At.YLimit;
            
            //d4n0n - подойдет ли вообще реализация двух игроков?
            
            bool gameOver = false;
            //Игровой цикл
            while (!gameOver)
            {
                stopwatch.Start();
                // В нём нужно выполнять действия по нажатию кнопок
                // Обновлять положения объектов
                // Определять столкновения и всё такое

                // Обработка нажатий клавиатуры
                if (Console.KeyAvailable)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var keyPressed = Console.ReadKey(true).Key;
                        while (Console.KeyAvailable)
                            Console.ReadKey(true);
                        if (keyPressed == ConsoleKey.Escape) break;

                        // Перемещение объектов по нажатию кнопок
                        players[i].Move(keyPressed);
                    }
                }

                for (int i = 0; i < ghosts.Count; i++)
                    for (int j=0; i < 2; i++)
                        if (players[j].X == ghosts[i].X && players[j].Y == ghosts[i].Y)
                            gameOver = true;

                //НУЖНО РЕАЛИЗОВАТЬ ДВИЖЕНИЕ ГОСТОВ


                // Вычисление задержки для поддержания стабильного FPS
                stopwatch.Stop();
                int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 0);
                // Задержка
                Thread.Sleep(sleepTime);
            }
            Console.Clear();
            Console.WriteLine("You've lost.");
            Console.ReadLine();
        }
    }
}
