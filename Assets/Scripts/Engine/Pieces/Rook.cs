using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using UnityEngine;
using Erebos.Engine.Utils;

namespace Erebos.Engine.Pieces
{
    public class Rook : Piece
    {
        public static readonly Vector2Int[] MovementVectors =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            return MovementVectors.SelectMany(movementVector => this.FindMovementsAlongPath(movementVector, startingCell));
        }
    }
}