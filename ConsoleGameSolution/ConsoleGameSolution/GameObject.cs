using System;

namespace ConsoleGameSolution
{
    public class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsStepped { get; set; }

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

        public static void WriteLevelNumber(int x, int y, string level, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(level);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

}
