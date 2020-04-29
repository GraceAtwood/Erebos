using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;

namespace Erebos.Engine.Pieces
{
    public class King : Piece
    {
        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    // Don't report that we can move on top of ourselves.
                    if (dx == 0 && dy == 0)
                        continue;

                    if (!startingCell.ChessBoard.TryGetCellFromPosition(startingCell.X + dx, startingCell.Y + dy, out var boardCell))
                        continue;

                    if (!boardCell.IsOccupied)
                    {
                        yield return new MoveTo(this, startingCell, boardCell);
                        continue;
                    }

                    if (boardCell.Piece.Side == Side.Opposite())
                        yield return new Attack(this, boardCell.Piece, startingCell, boardCell);
                }
            }

            foreach (var castleMove in FindCastlingMovesFrom(startingCell))
            {
                yield return castleMove;
            }
        }

        private IEnumerable<CastleMove> FindCastlingMovesFrom(ChessBoardCell startingCell)
        {
            if (HasMoved || this.IsInCheckAtPosition(startingCell))
                yield break;

            foreach (var rookStartingCell in new[] {0, 7}
                .Select(x => startingCell.ChessBoard.GetCellFromPosition(x, startingCell.Y))
                .Where(cell => cell.Piece is Rook rook && !rook.HasMoved))
            {
                var rookIsLeft = rookStartingCell.X < startingCell.X;
                var distance = Math.Abs(rookStartingCell.X - startingCell.X) - 1;

                var failure = false;
                for (var i = 1; i <= distance; i++)
                {
                    var x = startingCell.X + i * (rookIsLeft ? -1 : 1);
                    var boardCell = startingCell.ChessBoard.GetCellFromPosition(x, startingCell.Y);

                    if (boardCell.IsOccupied || i <= 2 && this.IsInCheckAtPosition(boardCell))
                    {
                        failure = true;
                        break;
                    }
                }

                // If we made it through that full loop, then we're good to castle!
                if (failure)
                    continue;

                var endingCell = startingCell.ChessBoard.GetCellFromPosition(rookIsLeft ? startingCell.X - 2 : startingCell.X + 2, startingCell.Y);
                var rookEndingCell = startingCell.ChessBoard.GetCellFromPosition(rookIsLeft ? endingCell.X + 1 : endingCell.X - 1, startingCell.Y);

                yield return new CastleMove(
                    this,
                    startingCell,
                    endingCell,
                    (Rook) rookStartingCell.Piece,
                    rookStartingCell,
                    rookEndingCell
                );
            }
        }

        public static bool IsInCheckAtPosition(Sides side, ChessBoardCell boardCell)
        {
            return boardCell.ChessBoard.CellsUnderAttack[side.Opposite()].Contains(boardCell);
        }
    }

    public static class KingExtensions
    {
        public static bool IsInCheckAtPosition(this King king, ChessBoardCell boardCell)
        {
            return King.IsInCheckAtPosition(king.Side, boardCell);
        }
    }
}