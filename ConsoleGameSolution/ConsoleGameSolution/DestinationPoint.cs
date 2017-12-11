using System;
namespace ConsoleGameSolution
{
    public class DestinationPoint : GameObject
    {
        public DestinationPoint Create()
        {                                                              //var random = new Random();
            X = Field.XLimit;                                         //random.Next(Field.XLimit) + 1;
            Y = 1;
            return this;
        }
    }
}
