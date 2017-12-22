using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Ghost : GameObject
    {
        public bool DirectedToRightSide;

        public List<Ghost> CreateGhosts(int countOfGhosts)
        {
            var ghosts = new List<Ghost>();
            var random = new Random();

            for (int i = 0; i < countOfGhosts; i++)
            {
                var xPos = random.Next(2, Field.XLimit-1);
                var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;
                var randomDirection = random.Next(2);

                foreach (var ghost in ghosts)
                    if (ghost.Y == yPos)
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

                ghosts.Add(new Ghost { X = xPos, Y = yPos, DirectedToRightSide = randomDirection == 1 ? true : false });
                WriteSymbol(ghosts[i].X, ghosts[i].Y, 'Y', ConsoleColor.DarkCyan);
            }

            return ghosts;
        }

        public void Move()
        {
            WriteSymbol(X, Y, ' ', ConsoleColor.White);

            if (DirectedToRightSide && X < Field.XLimit + 2)
                X++;
            if (X == Field.XLimit + 1) DirectedToRightSide = false;
            if (DirectedToRightSide == false && X > 0)
                X--;
            if (X == 1) DirectedToRightSide = true;

            WriteSymbol(X, Y, 'Y', ConsoleColor.Cyan);
        }
    }
}
