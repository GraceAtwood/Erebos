using System;
using System.Collections.Generic;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement;
using Erebos.Engine.GameManagement.Movement;
using UnityEngine;

namespace Erebos.Engine.Pieces
{
    public abstract class Piece : MonoBehaviour, IEquatable<Piece>
    {
        public Sides Side { get; set; }

        public bool HasMoved { get; set; }

        public abstract IEnumerable<AbstractMovement> GetPossibleMovementFrom(ChessBoardCell startingCell);

        public virtual void AnimateToCell(ChessBoardCell desiredChessBoardCell)
        {
            gameObject.transform.position = desiredChessBoardCell.WorldBoardPosition;
        }

        public virtual void Initialize(ChessBoardCell startingCell)
        {
            gameObject.transform.position = startingCell.WorldBoardPosition;

            Side = startingCell.Y <= 1 ? Sides.White : Sides.Black;
            gameObject.name = $"{GetType().Name}-{Side}";
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material =
                Side == Sides.Black ? startingCell.ChessBoard.blackPiecesMaterial : startingCell.ChessBoard.whitePiecesMaterial;
        }

        public void DestroyPiece()
        {
            gameObject.SetActive(false);
        }
        
        public void RecoverPiece()
        {
            gameObject.SetActive(true);
        }

        public void OnSelected()
        {
            Debug.Log($"{this} selected!");
        }

        public void OnDeselected()
        {
            Debug.Log($"{this} deselected!");
        }

        public bool Equals(Piece other)
        {
            return other != null && gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
        }

        public override string ToString()
        {
            return gameObject.name;
        }
    }
}