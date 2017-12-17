using System.IO;
using System;

namespace ConsoleGameSolution
{
    public class MapCreator
    {
        public static bool[,] CreateMap(string file) //поясни за класс
        {
            string[] readedMap = File.ReadAllLines(file);
            var finalMap = new bool[Console.BufferWidth, Console.BufferHeight - 2];

            for (int i = 0; i < readedMap.Length; i++)
                for (int j = 0; j < readedMap[0].Length; j++)
                    if (readedMap[i][j] == '#')
                    {
                        finalMap[j, i] = true;
                        Program.Objects.Add(new GameObject { X = j, Y = i });
                    }

            return finalMap;
        }
    }
}
