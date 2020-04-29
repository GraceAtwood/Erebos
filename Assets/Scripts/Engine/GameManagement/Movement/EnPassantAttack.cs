using System.Collections.Generic;
using Erebos.Engine.Pieces;

namespace Erebos.Engine.GameManagement.Movement
{
    public class EnPassantAttack : Attack
    {
        public ChessBoardCell DefenderCell { get; }

        public EnPassantAttack(Piece mover, Piece defender, ChessBoardCell startingCell, ChessBoardCell endingCell, ChessBoardCell defenderCell)
            : base(mover, defender, startingCell, endingCell)
        {
            DefenderCell = defenderCell;
        }

        protected override void DoExecute()
        {
            DefenderCell.Piece = null;

            base.DoExecute();
        }

        protected override void DoRollback()
        {
            DefenderCell.Piece = Defender;

            base.DoRollback();
        }
    }
}