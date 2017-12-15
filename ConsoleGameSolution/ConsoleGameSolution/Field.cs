using System;

namespace ConsoleGameSolution
{
    public class Field
    {
        public static int XLimit;
        public static int YLimit;

        public Field()
            : this(Console.WindowWidth -1 , Console.WindowHeight - 3)
        { }

        public Field(int xLimit, int yLimit)
        {
            XLimit = xLimit - 2;
            YLimit = yLimit - 2;
        }
    }
}
