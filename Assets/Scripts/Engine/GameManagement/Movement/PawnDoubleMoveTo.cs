using System;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public class PawnDoubleMoveTo : MoveTo
    {
        private Pawn Pawn { get; }

        public override bool PlacesEndingCellUnderAttack => false;

        public PawnDoubleMoveTo(Pawn pawn, ChessBoardCell startingCell, ChessBoardCell endingCell) 
            : base(pawn, startingCell, endingCell)
        {
            Pawn = pawn;
        }

        protected override void DoExecute()
        {
            Pawn.IsEnPassantEligible = true;
            
            base.DoExecute();
        }

        protected override void DoRollback()
        {
            Pawn.IsEnPassantEligible = false;
            
            base.DoRollback();
        }
    }
}