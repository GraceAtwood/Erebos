using System.Collections.Generic;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public abstract class AbstractMovement
    {
        protected AbstractMovement(Piece mover, ChessBoardCell startingCell, ChessBoardCell endingCell)
        {
            Mover = mover;
            StartingCell = startingCell;
            EndingCell = endingCell;
        }

        public Piece Mover { get; }
        public ChessBoardCell StartingCell { get; }
        public ChessBoardCell EndingCell { get; }

        public virtual bool PlacesEndingCellUnderAttack => true;

        protected bool RolledBack;

        public void Execute()
        {
            RolledBack = false;
            
            DoExecute();
        }

        protected abstract void DoExecute();

        public void Rollback()
        {
            RolledBack = true;
            
            DoRollback();
        }

        protected abstract void DoRollback();

        public IEnumerable<(Piece, ChessBoardCell)> FinalPositions()
        {
            yield return (Mover, RolledBack ? StartingCell : EndingCell);

            foreach (var additionalAffectedPiece in AdditionalFinalPositions())
            {
                yield return additionalAffectedPiece;
            }
        }

        protected virtual IEnumerable<(Piece, ChessBoardCell)> AdditionalFinalPositions()
        {
            yield break;
        }
    }
}