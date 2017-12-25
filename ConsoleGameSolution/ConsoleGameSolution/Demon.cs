using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Demon : GameObject
    {
        private bool DirectedToRightSide = false;

        public List<Demon> CreateDemons(int count)
        {
            //d4n0n: придумал реализацию - если demon находится в расстоянии < (сколько-нибудь) до игрока
            //включает demon mode -
            // 1) Прыгает к игроку на этаж
            // 2) игрок может спрятаться от demon, встав между этажами
            
            var demons = new List<Demon>();
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                var xPos = random.Next(1, Field.XLimit);
                var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;
                var randomDirection = random.Next(2);

                foreach (var obj in Program.Objects)
                    if (obj.Y == yPos && obj.X == xPos)
                    {
                        xPos = random.Next(1, Field.XLimit);
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;
                    }

                demons.Add(new Demon { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                Program.Objects.Add(new GameObject { X = xPos, Y = yPos });
                WriteSymbol(demons[i].X, demons[i].Y, 'X', ConsoleColor.Red);
                
            }

            return demons;
        }

        public void Move(Player player)
        {
            WriteSymbol(X, Y, ' ', ConsoleColor.White);
            var dirX = Math.Abs(player.X - this.X);
            var dirY = Math.Abs(player.Y - this.Y);
            var destinationToPlayer = Math.Sqrt(dirX*dirX + dirY*dirY);

            if (destinationToPlayer < 3)
                Y += player.Y.CompareTo(Y) * 2;

            if (DirectedToRightSide && X < Field.XLimit + 2)
                X++;
            if (X == Field.XLimit + 1) DirectedToRightSide = false;
            if (DirectedToRightSide == false && X > 0)
                X--;
            if (X == 1) DirectedToRightSide = true;

            WriteSymbol(X, Y, 'X', ConsoleColor.Red);
        }

    }
}
