using System.Collections.Generic;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;

namespace Erebos.Engine.Pieces
{
    public class Rook : Piece
    {
        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            yield break;
        }
    }
}