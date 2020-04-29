using System.Collections.Generic;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public class CastleMove : MoveTo
    {
        public override bool PlacesEndingCellUnderAttack => false;

        public CastleMove(King king, ChessBoardCell kingStartingCell, ChessBoardCell kingEndingCell, Rook rook, ChessBoardCell rookStartingCell, ChessBoardCell rookEndingCell)
            : base(king, kingStartingCell, kingEndingCell)
        {
            RookMoveTo = new MoveTo(rook, rookStartingCell, rookEndingCell);
        }

        private MoveTo RookMoveTo { get; }

        protected override void DoExecute()
        {
            RookMoveTo.Execute();
            
            base.DoExecute();
        }

        protected override void DoRollback()
        {
            RookMoveTo.Rollback();
            
            base.DoRollback();
        }

        protected override IEnumerable<(Piece, ChessBoardCell)> AdditionalFinalPositions()
        {
            return RookMoveTo.FinalPositions();
        }
    }
}