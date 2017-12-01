using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ConsoleGame
{
    /// <summary>
    /// Made by Danel Sibaev 11-706 and Zakharov Kirill 11-707
    /// </summary>
    class Program
    {
        public class At
        {
            private readonly int _xLimit;
            private readonly int _yLimit;
            public At()
                : this(Console.WindowWidth / 2, 0, Console.WindowWidth - 1, Console.WindowHeight - 2)
            { }

            public At(int x, int y, int xLimit, int yLimit)
            {
                X = 1;
                Y = 1;
                _xLimit = xLimit;
                _yLimit = yLimit;
                // Производим первичную отрисовку
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
            }
            public int X { get; private set; }
            public int Y { get; private set; }

            public void Move(ConsoleKey direction)
            {
                // Стираем следы (все пиксели игрового объекта на поле)
                Console.SetCursorPosition(X, Y);
                Console.Write(' ');
                switch (direction)
                {
                    case ConsoleKey.A:
                        if (X > 1) X--;
                        break;
                    case ConsoleKey.D:
                        if (X < _xLimit - 2) X++;
                        break;
                    case ConsoleKey.W:
                        if (Y > 1) Y--;
                        break;
                    case ConsoleKey.S:
                        if (Y < _yLimit - 1)
                            Y++;
                        break;
                }
                // Отрисовываем объект в новой позиции
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
            }
        }

        public class Ghost
        {
            public int X { get; set; }
            public int Y { get; set; }
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
                        Console.Write('+');
                    }

            //d4n0n - создать ghost'ов
            var ghosts = CreateGhosts();
            
            // Создаём игровые объекты
            var at = new At();

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
                    var keyPressed = Console.ReadKey(true).Key;
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    if (keyPressed == ConsoleKey.Escape) break;

                    // Перемещение объектов по нажатию кнопок
                    at.Move(keyPressed);
                }

                for (int i = 0; i < ghosts.Count; i++)
                    if (at.X == ghosts[i].X && at.Y == ghosts[i].Y)
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
