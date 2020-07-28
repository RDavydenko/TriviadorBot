using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.Util;


namespace TextRecogniser
{
	class Program
	{
		static void Main(string[] args)
		{
			string filePath = string.Empty;

			filePath = @"triviador_imgs\img_11.png";
			string currentDirectory = Directory.GetCurrentDirectory();

			bool imageExists = File.Exists(Path.Combine(currentDirectory,filePath));
			bool tessdataExists = File.Exists(@"C:\Users\Roman\source\repos\TriviadorBot\TextRecogniser\bin\Debug\tessdata\rus.traineddata");


			Tesseract tesseract = new Tesseract(@"C:\tessdata", "rus", OcrEngineMode.TesseractLstmCombined);

			var image = new Image<Bgr, byte>(filePath);
			tesseract.SetImage(image);

			tesseract.Recognize();

			string text = tesseract.GetUTF8Text();
			Console.WriteLine(text);

		}
	}
}
