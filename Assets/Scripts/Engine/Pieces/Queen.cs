using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using Erebos.Engine.Utils;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public class Queen : Piece
    {
        public static readonly Vector2Int[] MovementVectors =
            Bishop.MovementVectors.Concat(Rook.MovementVectors).ToArray();

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            return MovementVectors.SelectMany(movementVector => this.FindMovementsAlongPath(movementVector, startingCell));
        }
    }
}