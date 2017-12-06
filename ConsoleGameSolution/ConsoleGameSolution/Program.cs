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
                : this(Console.WindowWidth, Console.WindowHeight)
            { }

            public Field(int xLimit, int yLimit)
            {
                XLimit = xLimit;
                YLimit = yLimit;
            }
        }

        public class Object
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class Player : Object
        {
            public Player Create()
            {
                Player player = new Player { X = 1, Y = 1 };
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
                return player;
            }

            
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
                        if (X < Field.XLimit - 2) X++;
                        break;
                    case ConsoleKey.W:
                        if (Y > 0 && !Walls.walls[Y, X]) Y--;
                        break;
                    case ConsoleKey.S:
                        if (Y < Field.YLimit - 3 && !Walls.walls[Y, X])
                            Y++;
                        break;
                }

                // Отрисовываем объект в новой позиции
                Console.SetCursorPosition(X, Y);
                Console.Write('@');
            }

            public class Ghost : Object
            {
                public bool DirectedToRightSide;

                public List<Ghost> CreateGhosts()
                {
                    var ghosts = new List<Ghost>();
                    var random = new Random();
                    var countOfGhosts = random.Next(3,Field.YLimit / 2 + 1);
                    //d4n0n - создание ghosts

                    for (int i = 0; i < countOfGhosts; i++)
                    {
                        var xPos = random.Next(1, Field.XLimit - 2);
                        var yPos = random.Next(Field.YLimit / 2 - 1) * 2;
                        var randomDirection = random.Next(2);

                        foreach (var ghost in ghosts)
                            if (ghost.Y == yPos)
                            {
                                i--;
                                break;
                            }
                        
                        ghosts.Add(new Ghost {  X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                        Console.SetCursorPosition(ghosts[i].X, ghosts[i].Y);
                        Console.Write('2');
                    }

                    return ghosts;
                }

                public void Move()
                {
                    Console.SetCursorPosition(X, Y);
                    Console.Write(' ');

                    if (this.DirectedToRightSide == true && this.X < Field.XLimit - 2)
                        X++;
                    if (this.X == Field.XLimit - 3) DirectedToRightSide = false;
                    if (this.DirectedToRightSide == false && this.X > 1)
                        X--;
                    if (this.X == 2) DirectedToRightSide = true;

                    Console.SetCursorPosition(X, Y);
                    Console.Write('2');
                }
            }

            public class DestinationPoint : Object
            {
                public bool PlayerReachedDestinationPoint = false;
            }

            public class Walls
            {
                public static Boolean[,] walls = new bool[Field.YLimit, Field.XLimit];
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

                // Создаём игровые объекты
                var field = new Field();

                //d4n0n - Создать стены(пример обводки)
                for (int i = 0; i < Field.YLimit; i++)
                    for (int j = 0; j < Field.XLimit; j++)
                        if (i == 0 || j == 0 || i % 2 == 0 || j == Field.XLimit - 1)
                        {
                            Console.SetCursorPosition(j, i);
                            Console.Write('#');
                        }

                //d4n0n - player create
                Player player = new Player().Create();

                //d4n0n - создать ghost'ов
                var ghosts = new Ghost().CreateGhosts();
                
                //d4n0n - создание destination point
                DestinationPoint point = new DestinationPoint();
                var random = new Random();
                point.X = random.Next(Field.XLimit - 2) + 1;
                point.Y = Field.YLimit - 3;
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write("%");

                bool death = false;
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
                        player.Move(keyPressed);
                    }

                    // d4n0n - ДВИЖЕНИЕ ghosts
                    for (int i=0;i<ghosts.Count;i++)
                        ghosts[i].Move();

                    //d4n0n - проверка на столкновение
                    for (int i = 0; i < ghosts.Count - 1; i++)
                        for (int j = 0; i < 2; i++)
                            if (player.X == ghosts[i].X && player.Y == ghosts[i].Y)
                            {
                                gameOver = true;
                                death = true;
                            }

                    //d4n0n -  проверка на достижение цели
                    if (player.X == point.X && player.Y == point.Y)
                        gameOver = true;


                    // Вычисление задержки для поддержания стабильного FPS
                    stopwatch.Stop();
                    int sleepTime = Math.Max(frameDelay - (int)stopwatch.Elapsed.TotalMilliseconds, 0);
                    // Задержка
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
}
