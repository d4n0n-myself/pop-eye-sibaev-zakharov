using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Beast : GameObject
    {
        public List<Beast> CreateBeasts()
        {
            //d4n0n: придумал реализацию - если beast находится в расстоянии < (сколько-нибудь) до игрока
            //включает beast mode -
            // 1) Либо ломает стены насквозь
            // 2) Либо увеличивает скорость перемещения
            var beasts = new List<Beast>();
            var random = new Random();
            var countOfBeasts = random.Next(5, Field.YLimit / 2 + 1);
            return beasts;
        }
    }
}
