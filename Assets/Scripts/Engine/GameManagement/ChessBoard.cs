using System;
using System.Collections.Generic;
using System.Linq;
using Erebos.Engine.Enums;
using Erebos.Engine.GameManagement.Movement;
using Erebos.Engine.Pieces;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

namespace Erebos.Engine.GameManagement
{
    public class ChessBoard : MonoBehaviour, IInteractable
    {
        public GameObject rookPrefab;
        public GameObject pawnPrefab;
        public GameObject bishopPrefab;
        public GameObject queenPrefab;
        public GameObject kingPrefab;
        public GameObject knightPrefab;

        // Properties that hold the game's state
        public Sides CurrentTurn { get; private set; } = Sides.Black;

        public int TurnNumber { get; private set; } = 1;

        private ChessBoardCell[][] _boardCells;
        private Bounds _bounds;

        public Dictionary<Sides, Dictionary<Piece, List<AbstractMovement>>> PossibleMoves { get; } =
            new Dictionary<Sides, Dictionary<Piece, List<AbstractMovement>>>
            {
                {Sides.Black, new Dictionary<Piece, List<AbstractMovement>>()},
                {Sides.White, new Dictionary<Piece, List<AbstractMovement>>()}
            };

        public Dictionary<Sides, HashSet<ChessBoardCell>> CellsUnderAttack { get; } = new Dictionary<Sides, HashSet<ChessBoardCell>>
        {
            {Sides.Black, new HashSet<ChessBoardCell>()},
            {Sides.White, new HashSet<ChessBoardCell>()}
        };

        public Dictionary<Sides, bool> KingsInCheck { get; } = new Dictionary<Sides, bool>
        {
            {Sides.Black, false},
            {Sides.White, false}
        };

        public Piece SelectedPiece { get; private set; }

        // Events related to game state changes
        public event EventHandler<TurnEndedEventArgs> TurnEnded;
        public event EventHandler<TurnEndingEventArgs> TurnEnding;

        public event EventHandler<PieceDestroyedEventArgs> PieceDestroyed;

        public Vector2 TileSize { get; private set; }
        public Vector3 LocalMeshMin { get; private set; }
        public Vector3 LocalMeshMax { get; private set; }

        void Start()
        {
            _bounds = GetComponent<Renderer>().bounds;
            LocalMeshMin = transform.InverseTransformPoint(_bounds.min);
            LocalMeshMax = transform.InverseTransformPoint(_bounds.max);

            var distanceX = Math.Abs((LocalMeshMax - LocalMeshMin).x / 8);
            var distanceY = Math.Abs((LocalMeshMax - LocalMeshMin).y / 8);

            TileSize = new Vector2(distanceX, distanceY);

            InitializeCells();
            InitializePieces();
            RecalculateGameState();
        }

        public void EndTurn()
        {
            TurnEnding?.Invoke(this, new TurnEndingEventArgs(CurrentTurn));
            CurrentTurn = CurrentTurn.Opposite();
            TurnNumber++;
            SelectedPiece.OnDeselected();
            SelectedPiece = null;
            TurnEnded?.Invoke(this, new TurnEndedEventArgs());
        }

        private bool TryMovement(AbstractMovement movement)
        {
            if (movement == null)
                return false;
            
            movement.Execute();
            
            RecalculateGameState();

            if (!KingsInCheck[movement.Mover.Side]) 
                return true;
            
            // If this was a bad movement, roll back.
            Debug.Log("Rolling back!");
            movement.Rollback();
            RecalculateGameState();
            return false;
        }

        private void InitializeCells()
        {
            var boardCells = new ChessBoardCell[8][];
            for (var rowNumber = 0; rowNumber < 8; rowNumber++)
            {
                var row = new ChessBoardCell[8];
                for (var columnNumber = 0; columnNumber < 8; columnNumber++)
                {
                    row[columnNumber] = new ChessBoardCell(rowNumber, columnNumber, this);
                }

                boardCells[rowNumber] = row;
            }

            _boardCells = boardCells;
        }

        private void InitializePieces()
        {
            // Pawns
            foreach (var y in new[] {1, 6})
                for (var x = 0; x <= 7; x++)
                    InstantiatePieceAt(pawnPrefab, x, y);

            // Rooks
            foreach (var x in new[] {0, 7})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt(rookPrefab, x, y);

            // Knights
            foreach (var x in new[] {1, 6})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt(knightPrefab, x, y);

            // Bishops
            foreach (var x in new[] {2, 5})
            foreach (var y in new[] {0, 7})
                InstantiatePieceAt(bishopPrefab, x, y);

            // Queens
            InstantiatePieceAt(queenPrefab, 3, 0);
            InstantiatePieceAt(queenPrefab, 4, 7);

            // Kings
            InstantiatePieceAt(kingPrefab, 4, 0);
            InstantiatePieceAt(kingPrefab, 3, 7);
        }

        private void InstantiatePieceAt(GameObject piecePrefab, int x, int y)
        {
            var piece = Instantiate(piecePrefab, gameObject.transform).GetComponent<Piece>();

            var boardCell = _boardCells[x][y];
            boardCell.Piece = piece;
            piece.Initialize(boardCell);
        }

        private ChessBoardCell GetCellFromWorldPoint(Vector3 position)
        {
            var length = _bounds.max - _bounds.min;
            var tileSizeX = length.x / 8;
            var tileSizeZ = length.z / 8;

            var distanceX = Math.Abs(position.x - _bounds.min.x);
            var distanceZ = Math.Abs(position.z * -1 - _bounds.min.z);

            var posX = (int) (distanceX / tileSizeX);
            var posZ = (int) (distanceZ / tileSizeZ);

            var result = GetCellFromPosition(posX, posZ);

            return result;
        }

        public ChessBoardCell GetCellFromPosition(int x, int y)
        {
            return _boardCells[x][y];
        }

        public bool TryGetCellFromPosition(int x, int y, out ChessBoardCell chessBoardCell)
        {
            chessBoardCell = null;

            if (x < 0 || x > 7 || y < 0 || y > 7)
                return false;

            chessBoardCell = _boardCells[x][y];
            return true;
        }

        private void RecalculateGameState()
        {
            foreach (var side in PossibleMoves.Keys)
                PossibleMoves[side].Clear();
            
            foreach (var side in KingsInCheck.Keys.ToList())
                KingsInCheck[side] = false;
            
            foreach (var side in CellsUnderAttack.Keys)
                CellsUnderAttack[side].Clear();
            
            foreach (var boardCell in _boardCells
                .SelectMany(x => x)
                .Where(x => x.IsOccupied))
            {
                PossibleMoves[boardCell.Piece.Side][boardCell.Piece] = new List<AbstractMovement>();
                
                foreach (var move in boardCell.Piece.GetPossibleMovementFrom(boardCell))
                {
                    PossibleMoves[boardCell.Piece.Side][boardCell.Piece].Add(move);

                    if (!move.PlacesEndingCellUnderAttack) 
                        continue;

                    CellsUnderAttack[boardCell.Piece.Side].Add(move.EndingCell);
                    
                    if (move is Attack attack && attack.Defender is King king)
                        KingsInCheck[king.Side] = true;
                }
            }
        }

        public void OnPrimaryMouseUp(MouseEventArgs mouseEventArgs)
        {
            var boardCell = GetCellFromWorldPoint(mouseEventArgs.Point);

            if (SelectedPiece == null)
            {
                // We know we're going to try to make a selection
                // Is the cell empty or an enemy piece?
                if (!boardCell.IsOccupied || boardCell.Piece.Side != CurrentTurn)
                    return;

                // It must be our piece!
                SelectedPiece = boardCell.Piece;
                SelectedPiece.OnSelected();
            }
            else
            {
                // Here we have a piece selected so the player is trying to do something with that piece or change the selection.
                if (SelectedPiece.Equals(boardCell.Piece))
                {
                    SelectedPiece.OnDeselected();
                    SelectedPiece = null;
                    return;
                }

                var possibleMovesForSelectedPiece = PossibleMoves[SelectedPiece.Side][SelectedPiece];

                var movementToCell = possibleMovesForSelectedPiece.SingleOrDefault(x => x.EndingCell.Equals(boardCell));
                
                if (movementToCell == null || !TryMovement(movementToCell))
                {
                    SelectedPiece.OnDeselected();
                    SelectedPiece = null;
                    return;
                }

                foreach (var piece in movementToCell.FinalPositions().Select(x => x.Item1))
                {
                    piece.HasMoved = true;
                }

                EndTurn();
            }
        }

        public void OnPrimaryMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnSecondaryMouseUp(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnSecondaryMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        public void OnMouseHover(MouseEventArgs mouseEventArgs)
        {
        }
    }
}