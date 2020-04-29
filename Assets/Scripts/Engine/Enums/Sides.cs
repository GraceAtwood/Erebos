﻿using System;

 namespace Erebos.Engine.Enums
{
    public enum Sides
    {
        Black,
        White
    }

    public static class SidesExtensions
    {
        public static Sides Opposite(this Sides side)
        {
            switch (side)
            {
                case Sides.Black:
                    return Sides.White;
                case Sides.White:
                    return Sides.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}