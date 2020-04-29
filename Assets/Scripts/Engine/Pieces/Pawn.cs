using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;

namespace Erebos.Engine.Pieces
{
    public class Pawn : Piece
    {
        public bool IsEnPassantEligible { get; set; }

        private int _deltaForward;

        private void TurnEndingHandler(object sender, TurnEndingEventArgs e)
        {
            // If the enemy is finishing their turn and we're still alive, we're not eligible anymore!
            if (e.CurrentSide == Side.Opposite())
                IsEnPassantEligible = false;
        }

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            // Check basic forward movement
            if (startingCell.ChessBoard.TryGetCellFromPosition(startingCell.X, startingCell.Y + _deltaForward, out var boardCell) &&
                !boardCell.IsOccupied)
            {
                yield return new MoveTo(this, startingCell, boardCell);
            }

            // Check double forward movement
            if (!HasMoved && startingCell.ChessBoard.TryGetCellFromPosition(startingCell.X, startingCell.Y + _deltaForward * 2,
                out var boardCellPassant) && !boardCell.IsOccupied)
            {
                yield return new PawnDoubleMoveTo(this, startingCell, boardCellPassant);
            }

            // Can I attack to the side normally?  And am I adjacent to an en passant eligible piece?
            foreach (var dx in new[] {-1, 1})
            {
                if (startingCell.ChessBoard.TryGetCellFromPosition(startingCell.X + dx, startingCell.Y + _deltaForward, out var boardCellAttack) &&
                    boardCellAttack.IsOccupied && boardCellAttack.Piece.Side == Side.Opposite())
                    yield return new Attack(this, boardCellAttack.Piece, startingCell, boardCellAttack);
                else if (startingCell.ChessBoard.TryGetCellFromPosition(startingCell.X + dx, startingCell.Y, out var boardCellEnPassantCheck) && boardCellEnPassantCheck.Piece is Pawn pawn && pawn.IsEnPassantEligible)
                    yield return new EnPassantAttack(this, boardCellEnPassantCheck.Piece, startingCell, boardCellAttack, boardCellEnPassantCheck);
            }
        }

        public override void Initialize(ChessBoardCell startingCell)
        {
            switch (startingCell.Y)
            {
                case 1:
                    _deltaForward = 1;
                    break;
                case 6:
                    _deltaForward = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(startingCell.Y), startingCell.Y,
                        $"Attempted to initialize a pawn to a Y position that wasn't 1 or 6.  And that doesn't make sense.");
            }

            startingCell.ChessBoard.TurnEnding += TurnEndingHandler;

            base.Initialize(startingCell);
        }
    }
}