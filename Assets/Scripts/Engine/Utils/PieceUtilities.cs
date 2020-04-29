using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using Erebos.Engine.Pieces;
using UnityEngine;

namespace Erebos.Engine.Utils
{
    public static class PieceUtilities
    {
        public static IEnumerable<AbstractMovement> FindMovementsAlongPath(this Piece piece, Vector2Int movementVector, ChessBoardCell startingCell)
        {
            for (var i = 1; i <= 7; i++)
            {
                var x = startingCell.X + movementVector.x * i;
                var y = startingCell.Y + movementVector.y * i;

                if (!startingCell.ChessBoard.TryGetCellFromPosition(x, y, out var chessBoardCell))
                    break;

                if (!chessBoardCell.IsOccupied)
                {
                    yield return new MoveTo(piece, startingCell, chessBoardCell);
                    continue;
                }

                if (chessBoardCell.Piece.Side == piece.Side)
                    break;

                if (chessBoardCell.Piece.Side == piece.Side.Opposite())
                {
                    yield return new Attack(piece, chessBoardCell.Piece, startingCell, chessBoardCell);
                    break;
                }
            }
        }
    }
}