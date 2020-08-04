using System;
using System.Drawing;

namespace ConsoleRecogniser.Models
{
	class CutterFHD : ICutter
	{
		private Bitmap Cut(Bitmap src, Rectangle rect)
		{
			Bitmap bmp = new Bitmap(src.Width, src.Height);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(src, 0, 0, rect, GraphicsUnit.Pixel);
			return bmp;
		}

		public Bitmap CutQuestionText(Bitmap src)
		{
			int questionX = 491;
			int questionY = 364;
			int questionWidth = 604;
			int questionHeigth = 118;
			Rectangle rect = new Rectangle(new Point(questionX, questionY), new Size(questionWidth, questionHeigth)); // Вопрос
			return Cut(src, rect);
		}

		public Bitmap CutQuestionVariant(Bitmap src, int number)
		{
			if (number < 1 || number > 4)
				throw new Exception("Доступно вырезать только с 1 по 4 вопрос, включая границы. Неверный параметр number");


			int firstRectLeftTopCoordX = 575;
			int firstRectLeftTopCoordY = 507;
			int between = 20;
			int varWidth = 443;
			int varHeigth = 62;
			Rectangle rect = new Rectangle(new Point(firstRectLeftTopCoordX, firstRectLeftTopCoordY + (number - 1) * (varHeigth + between)), new Size(varWidth, varHeigth));
			return Cut(src, rect);
		}

		public Bitmap CutNumericText(Bitmap src)
		{
			Rectangle rect = new Rectangle(new Point(506, 365), new Size(575, 109)); // Вопрос			
			return Cut(src, rect);
		}

		public Bitmap CutNumericAnswer(Bitmap src)
		{
			Rectangle rect = new Rectangle(new Point(678, 485), new Size(246, 60)); // Ответ
			return Cut(src, rect);
		}
	}
}
