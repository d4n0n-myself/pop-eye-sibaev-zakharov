using System.IO;
using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class MapCreator
    {
        public static bool[,] CreateMap(string file) //поясни за класс
        {
            string[] readedMap = File.ReadAllLines(file);
            var finalMap = new bool[Console.BufferWidth, Console.BufferHeight - 2];

            for (int i = 0; i < readedMap.Length - 1; i++)
                for (int j = 0; j < readedMap[0].Length; j++)
                    if (readedMap[i][j] == '#')
                    {
                        finalMap[j, i] = true;
                        Program.Objects.Add(new GameObject { X = j, Y = i });
                    }

            return finalMap;


        }
        public static Dictionary<string, int> GameObjects(string file) //Kirill: возможность конфигурации количества объектов на уровне в файле уровня.
        {
            var objects = new Dictionary<string, int>();
            string count = "";
            string[] readedMap = File.ReadAllLines(file);
            var lastline = readedMap.Length-1;
            var tempLetter = ' ';
            for (int j = 0; j < readedMap[lastline].Length; j++)
            {
                if (Char.IsLetter(readedMap[lastline][j]))
                {
                    if (count != "")
                    {
                        objects[tempLetter.ToString()] = Convert.ToInt32(count);
                        count = "";
                    }
                    tempLetter = (readedMap[lastline][j]);
                    objects.Add(readedMap[lastline][j].ToString(), 0);
                    
                }
                else if (Char.IsDigit(readedMap[lastline][j]))
                    count += readedMap[lastline][j];

            }
            objects[readedMap[lastline][readedMap[lastline].Length - 1].ToString()] = Convert.ToInt32(count);
            return objects;
        }
    }
}
