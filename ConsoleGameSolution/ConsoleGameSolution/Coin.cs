using System;
using System.Collections.Generic;


namespace ConsoleGameSolution
{
    public class Coin : GameObject
    {
        public List<Coin> CreateCoins(int count)
        {
            // Kiri: это очки для HighScore
            var coins = new List<Coin>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var xPos = random.Next(1, Field.XLimit);
                var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;


                foreach (var obj in Program.Objects)
                    if (obj.Y == yPos&& obj.X == xPos)
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

                coins.Add(new Coin { X = xPos, Y = yPos, IsStepped = false, color = ConsoleColor.DarkYellow, Symbol = '$' });
                Program.Objects.Add(new GameObject { X = xPos, Y = yPos });
            }

            return coins;
        }
    }
}
