using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Go
{
	public enum Piece
	{
		Empty,
		Black,
		White
	}
	public class GoMove : MoveBase
	{
		public Piece Piece { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
        public GoMove(Piece piece, int x, int y)
        {
			Piece = piece;
			X = x;
			Y = y;
        }
    }
	public class GoModel: GameModelBase
	{
		public Piece[,] Chessboard { get; set; } = new Piece[19, 19];
		public Piece WhoseTurn { get; set; }
		public Piece WhoIsAI { get; set;}
		public static Piece EnemyOf(Piece stone)
		{
			if (stone == Piece.Black)
			{
				return Piece.White;
			}
			else if(stone == Piece.White)
			{
				return Piece.Black;
			}
			return Piece.Empty;
		}
		public void SelectBlock(Stack<GoMove> selectedPieces, GoMove testMove)
		{
			selectedPieces.Push(testMove);

			GoMove[] potentialTestMove = new GoMove[4];
			potentialTestMove[0] = new GoMove(testMove.Piece, testMove.X + 1, testMove.Y);
			potentialTestMove[1] = new GoMove(testMove.Piece, testMove.X, testMove.Y + 1);
			potentialTestMove[2] = new GoMove(testMove.Piece, testMove.X - 1, testMove.Y);
			potentialTestMove[3] = new GoMove(testMove.Piece, testMove.X, testMove.Y - 1);

			for (int i = 0; i < 4; ++i)
			{
				if (selectedPieces.Contains(potentialTestMove[i]))
				{
					continue;
				}
				if (Chessboard[potentialTestMove[i].Y, potentialTestMove[i].X] == testMove.Piece)
				{
					SelectBlock(selectedPieces, potentialTestMove[i]);
				}
			}
		}
		public bool haveQi(GoMove goMove)
		{
			Stack<GoMove> pieceBlock = new ();
			SelectBlock(pieceBlock, goMove);
			while (pieceBlock.Count > 0)
			{
				GoMove testMove = pieceBlock.Pop();
				bool haveEmptyNeighboor = Chessboard[testMove.Y + 1, testMove.X] == Piece.Empty
										|| Chessboard[testMove.Y, testMove.X + 1] == Piece.Empty
										|| Chessboard[testMove.Y - 1, testMove.X] == Piece.Empty
										|| Chessboard[testMove.Y, testMove.X - 1] == Piece.Empty;
                if (haveEmptyNeighboor)
                {
                    return true;
                }
            }
			return false;
		}
		public override bool ValidMove(MoveBase move)
		{
			GoMove goMove = (GoMove)move;
			if (Chessboard[goMove.Y, goMove.X] == Piece.Empty && haveQi(goMove))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public override void DoMove(MoveBase move)
		{
			GoMove goMove = (GoMove)move;
			Chessboard[goMove.Y, goMove.X] = goMove.Piece;


            GoMove[] potentialEliminatedPieces = new GoMove[4];
            potentialEliminatedPieces[0] = new GoMove(goMove.Piece, goMove.X + 1, goMove.Y);
            potentialEliminatedPieces[1] = new GoMove(goMove.Piece, goMove.X, goMove.Y + 1);
            potentialEliminatedPieces[2] = new GoMove(goMove.Piece, goMove.X - 1, goMove.Y);
            potentialEliminatedPieces[3] = new GoMove(goMove.Piece, goMove.X, goMove.Y - 1);

			for (int i = 0; i < 4; ++i)
			{
				if (Chessboard[potentialEliminatedPieces[i].Y, potentialEliminatedPieces[i].X] == EnemyOf(goMove.Piece))
				{
					if (haveQi(potentialEliminatedPieces[i]) == false)
					{
						Stack<GoMove> selectedPiecesToBeEliminated = new();
						SelectBlock(selectedPiecesToBeEliminated, potentialEliminatedPieces[i]);
						while (selectedPiecesToBeEliminated.Count > 0) {
							GoMove pieceToBeEliminated = selectedPiecesToBeEliminated.Pop();
							Chessboard[pieceToBeEliminated.Y, pieceToBeEliminated.X] = Piece.Empty;
						}
					}
				}
			}
				
            WhoseTurn = GoModel.EnemyOf(WhoseTurn);
		}
		public override GameModelBase Copy()
		{
			GoModel model = new GoModel();
			model.WhoseTurn = WhoseTurn;
			model.WhoIsAI = WhoIsAI;
			for(int i = 0; i < 19; i++)
			{
				for(int j = 0; j < 19; j++)
				{
					model.Chessboard[i,j] = Chessboard[i,j];
				}
			}
			return model;
		}
        public GoModel()
        {
			WhoseTurn = Piece.Black;
        }
    }
}
