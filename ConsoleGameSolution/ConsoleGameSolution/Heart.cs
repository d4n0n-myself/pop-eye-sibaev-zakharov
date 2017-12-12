using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Heart : GameObject
    {
        //KIRI:добавочные жизни игроку
        //лежат на карте, игрок подбирает и LivesCount++;
        public List<Heart> CreateHearts()
        {
            var hearts = new List<Heart>();
            var random = new Random();
            var countOfHearts = random.Next(0, 2);
            return hearts;
        }
    }
}
