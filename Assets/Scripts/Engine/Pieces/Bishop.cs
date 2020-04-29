using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using Erebos.Engine.Utils;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public class Bishop : Piece
    {
        public static readonly Vector2Int[] MovementVectors = {
            new Vector2Int(1, 1), 
            new Vector2Int(1, -1), 
            new Vector2Int(-1, 1), 
            new Vector2Int(-1, -1) 
        };

        public override IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell)
        {
            return MovementVectors.SelectMany(movementVector => this.FindMovementsAlongPath(movementVector, startingCell));
        }
    }
}