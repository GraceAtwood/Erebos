﻿using Erebos.Engine.Enums;

 namespace Erebos.Engine.GameManagement
{
    public class TurnEndingEventArgs
    {
        public TurnEndingEventArgs(Sides currentSide)
        {
            CurrentSide = currentSide;
            NextSide = currentSide.Opposite();
        }

        public Sides CurrentSide { get; }
        public Sides NextSide { get; }
    }
}