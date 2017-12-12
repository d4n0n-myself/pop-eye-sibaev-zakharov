using System;
using System.Collections.Generic;
using System.Text;

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
            var count = random.Next(1, 2);
            for (int i = 0; i < count; i++)
            {
                var xPos = random.Next(1, Field.XLimit);
                var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;


                foreach (var heart in hearts)
                    if (heart.Y == yPos)
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

                hearts.Add(new Heart { X = xPos, Y = yPos, IsStepped = false, color = ConsoleColor.Magenta, Symbol = 'H' });
            
            }
            return hearts;
        }
    }
}
