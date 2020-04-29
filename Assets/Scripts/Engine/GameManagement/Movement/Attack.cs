using System.Collections.Generic;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public class Attack : AbstractMovement
    {
        public Piece Defender { get; }

        public Attack(Piece mover, Piece defender, ChessBoardCell startingCell, ChessBoardCell endingCell) 
            : base(mover, startingCell, endingCell)
        {
            Defender = defender;
        }

        protected override void DoExecute()
        {
            StartingCell.Piece = null;
            EndingCell.Piece = Mover;
            
            Mover.AnimateToCell(EndingCell);
            Defender.DestroyPiece();
        }

        protected override void DoRollback()
        {
            StartingCell.Piece = Mover;
            EndingCell.Piece = null;
            
            Mover.AnimateToCell(StartingCell);
            Defender.RecoverPiece();
        }

        protected override IEnumerable<(Piece, ChessBoardCell)> AdditionalFinalPositions()
        {
            yield return (Defender, RolledBack ? EndingCell : StartingCell);
        }
    }
}