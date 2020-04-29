using System;
using Erebos.Engine.Pieces;
using UnityEngine;
using UnityEngine.AI;

namespace Erebos.Engine.GameManagement
{
    public class ChessBoardCell : IEquatable<ChessBoardCell>
    {
        public ChessBoard ChessBoard { get; }

        public int X { get; }
        
        public int Y { get; }

        public Vector3 WorldBoardPosition { get; }

        public Piece Piece { get; set; }

        public bool IsOccupied => Piece != null;

        public ChessBoardCell(int x, int y, ChessBoard chessBoard)
        {
            if (x < 0 || x > 7)
                throw new ArgumentOutOfRangeException(nameof(x), x, "X must be in the range [0, 7]");

            if (y < 0 || y > 7)
                throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be in the range [0, 7]");

            ChessBoard = chessBoard;
            X = x;
            Y = y;

            var bounds = ChessBoard.GetComponent<Renderer>().bounds;

            var length = bounds.max - bounds.min;
            var tileSizeX = length.x / 8;
            var tileSizeZ = length.z / 8;

            var moveX = tileSizeX * X + tileSizeX / 2;
            var moveZ = tileSizeZ * Y + tileSizeZ / 2;
            
            var position = new Vector3(bounds.min.x + moveX, bounds.max.y, bounds.min.z + moveZ);

            WorldBoardPosition = position;
        }

        public bool Equals(ChessBoardCell other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}