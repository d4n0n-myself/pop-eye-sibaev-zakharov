using System;
namespace ConsoleGameSolution
{
    public class DestinationPoint : GameObject
    {
        public DestinationPoint Create()
        {                                                              
            X = Field.XLimit;                                         
            Y = 1;
            return this;
        }
    }
}
