using System.IO;

namespace ConsoleGameSolution
{
    public class MapCreator
    {
        public static bool[,] CreateMap(string file)
        {
            var finalMap = new bool[Field.XLimit + 2, Field.YLimit + 2];
            string[] readedMap = File.ReadAllLines(file);

            for (int i = 0; i < Field.XLimit + 2; i++)
                for (int j = 0; j < Field.YLimit + 2; j++)
                    if (readedMap[i][j] == '#')
                        finalMap[i, j] = true;

            return finalMap;
        }
    }
}
