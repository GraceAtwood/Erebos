using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public class Bishop : Piece
    {
        private static readonly Vector2Int[] MovementVectors = {
            new Vector2Int(1, 1), 
            new Vector2Int(1, -1), 
            new Vector2Int(-1, 1), 
            new Vector2Int(-1, -1) 
        };

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            foreach (var movementVector in MovementVectors)
            {
                for (var i = 1; i <= 7; i++)
                {
                    var x = startingCell.X + movementVector.x * i;
                    var y = startingCell.Y + movementVector.y * i;

                    if (!startingCell.ChessBoard.TryGetCellFromPosition(x, y, out var chessBoardCell))
                        break;

                    if (!chessBoardCell.IsOccupied)
                    {
                        yield return new MoveTo(this, startingCell, chessBoardCell);
                        continue;
                    }

                    if (chessBoardCell.Piece.Side == Side)
                        break;

                    if (chessBoardCell.Piece.Side == Side.Opposite())
                    {
                        yield return new Attack(this, chessBoardCell.Piece, startingCell, chessBoardCell);
                        break;
                    }
                }
            }
        }
    }
}