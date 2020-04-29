using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public class Knight : Piece
    {
        private static readonly Vector2Int[] MovementVectors =
        {
            new Vector2Int(1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(-1, -2),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
        };

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            foreach (var movementVector in MovementVectors)
            {
                if (!startingCell.ChessBoard.TryGetCellFromPosition(
                    startingCell.X + movementVector.x, startingCell.Y + movementVector.y, out var boardCell)) 
                    continue;
                
                if (!boardCell.IsOccupied)
                    yield return new MoveTo(this, startingCell, boardCell);
                else if (boardCell.Piece.Side == Side.Opposite())
                    yield return new Attack(this, boardCell.Piece, startingCell, boardCell);
            }
        }
    }
}