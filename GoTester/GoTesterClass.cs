﻿using Contracts;
using Contracts.BaseClasses;
using Emgu.CV;
using Emgu.CV.ML.MlEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Drawing2D;
using Go;
using GoTester.Properties;
namespace GoTester
{
	public class GoTesterClass: TesterBase
	{
		public GoModel Model { get; } = new GoModel();
		Image<Bgr, byte> Image { get; set; } = new Image<Bgr, byte>(30, 30);
		private void PutPixel(int x, int y, byte r, byte g, byte b)
		{
			Image.Data[y, x, 0] = b;
			Image.Data[y, x, 1] = g;
			Image.Data[y, x, 2] = r;
		}
		//private void FillRectangle(int x1, int y1, int x2,  int y2, byte r, byte g, byte b)
		//{
		//	for(int i = y1; i < y2;i++)
		//	{
		//		for(int j = x1; j < x2;j++)
		//		{
		//			PutPixel(i, j, r, g, b);
		//		}
		//	}
		//}
		private void DrawSprite(Image<Bgr, byte> sprite, int x, int y)
		{
			byte[,,] data = sprite.Data;
			int rowCount = data.GetLength(0);
			int columnCount = data.GetLength(1);
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < columnCount; j++)
				{
					PutPixel(x + j, y + i, data[j, i, 2], data[j, i, 1], data[j, i, 0]);
				}
			}
		}
		private void DrawBoard()
		{
			Image = Resources.board.ToImage<Bgr, byte>();
			for(int i = 0;i < 19;i++)
			{
				for(int j = 0;j < 19;j++)
				{
					if (Model.Chessboard[i,j] == Piece.Black)
					{
						DrawSprite(Resources.black.ToImage<Bgr, byte>(), 27 * j + 4, 27 * i + 4);
					}
					else if (Model.Chessboard[i,j] == Piece.White)
					{
						DrawSprite(Resources.white.ToImage<Bgr, byte>(), 27 * j + 4, 27 * i + 4);
					}
				}
			}
		}
		public void Init()
		{
			DrawBoard();
			UpdateImage(Image);
		}
		public override void OnLeftButtonDown(double x, double y)
		{
			int realX = (int)(x * 19.0);
			int realY = (int)(y * 19.0);
			if (realX > 2 || realY > 2)
			{
				return;
			}
			GoMove move = new GoMove(Model.WhoseTurn, realX, realY);
			bool validMove = Model.ValidMove(move);
			if (validMove)
			{
				Model.DoMove(move);
				SendMessage($"成功执行了开发者的落子({move.Piece}, {realX}, {realY})");
				DrawBoard();
				UpdateImage(Image);
				try
				{
					MoveInfo info = ProjSlnFuncProvider!.Execute(Model);
					if (info.Succeeded)
					{
						SendMessage($"AI落子成功");
						GoMove aiMove = (GoMove)info.Move!;
						SendMessage($"AI的落子: ({aiMove.Piece}, {aiMove.X}, {aiMove.Y})");
						if (Model.ValidMove(aiMove))
						{
							Model.DoMove(aiMove);
							DrawBoard();
							UpdateImage(Image);
							SendMessage($"AI落子合规");
						}
						else
						{
							SendMessage("错误: AI落子不合规");
						}
					}
					else
					{
						SendMessage("AI无法决策如何落子");
					}
				}
				catch (Exception e)
				{
					SendMessage("调用AI时引发的异常: " + e.Message);
				}
			}
			else
			{
				SendMessage("非法的移动");
			}
		}
		public override void OnRightButtonDown(double x, double y)
		{
			throw new NotImplementedException();
		}
		public override void OnReceiveMessage(string message)
		{
			SendMessage($"收到了指令: {message}");
			if (message == "clear")
			{
				Model.Chessboard = new Piece[3, 3];
				DrawBoard();
				UpdateImage(Image);
				SendMessage("收到了重置指令");
			}
		}
		public GoTesterClass()
        {
            
        }
    }
}