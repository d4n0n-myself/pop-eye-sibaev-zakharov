using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Beast : GameObject
    {
        public bool DirectedToRightSide = false;
        public Beast CreateBeast()
        {
            //d4n0n: придумал реализацию - если beast находится в расстоянии < (сколько-нибудь) до игрока
            //включает beast mode -
            // 1) Прыгает к игроку на этаж
            // 2) игрок может спрятаться от beast, встав между этажами
            
            var beast = new Beast();
            var random = new Random();

            var xPos = random.Next(1, Field.XLimit);
            var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

            foreach (var obj in Program.Objects)
                if (obj.Y == yPos && obj.X == xPos)
                    yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

            Program.Objects.Add(new GameObject { X = xPos, Y = yPos });
            beast.X = xPos;
            beast.Y = yPos;
            beast.color = ConsoleColor.Red;
            beast.Symbol = 'X';

            return beast;
        }

        public Beast BeastMove(Player player)
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

            return new Beast { X = X, Y = Y, Symbol = 'X', color = ConsoleColor.Red };
        }

    }
}
