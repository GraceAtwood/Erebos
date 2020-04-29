using System.Collections.Generic;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public class MoveTo : AbstractMovement
    {
        public override bool PlacesEndingCellUnderAttack => !(Mover is Pawn);
        
        public MoveTo(Piece mover, ChessBoardCell startingCell, ChessBoardCell endingCell)
            : base(mover, startingCell, endingCell)
        {
        }

        protected override void DoExecute()
        {
            StartingCell.Piece = null;
            EndingCell.Piece = Mover;
            
            Mover.AnimateToCell(EndingCell);
        }

        protected override void DoRollback()
        {
            StartingCell.Piece = Mover;
            EndingCell.Piece = null;
            
            Mover.AnimateToCell(StartingCell);
        }
    }
}