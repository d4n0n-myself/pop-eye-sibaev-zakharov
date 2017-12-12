using System;
using System.Collections.Generic;

namespace ConsoleGameSolution
{
    public class Button : GameObject
    {
        
        
        
        public List<Button> CreateButtons(int count)
        {
            var buttons = new List<Button>();
            var random = new Random();
            

            for (int i = 0; i < count; i++)
            {
                var xPos = random.Next(1, Field.XLimit);
                var yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;


                foreach (var button in buttons)
                    if (button.Y == yPos)
                        yPos = random.Next(1, Field.YLimit / 2) * 2 + 1;

                buttons.Add(new Button { X = xPos, Y = yPos, IsStepped = false, color = ConsoleColor.Cyan , Symbol='B' });
            }

            return buttons;
        }
    }
}
