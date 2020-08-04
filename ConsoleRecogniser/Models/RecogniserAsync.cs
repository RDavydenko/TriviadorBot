using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;

namespace ConsoleRecogniser.Models
{
	class RecogniserAsync : IAsyncRecogniser
	{
		private Tesseract _tesseract;

		public RecogniserAsync(string lang = "rusf")
		{
			string dataPath = "tessdata"; // Папка, где лежат traineddata
			OcrEngineMode mode = lang == "rusf" ? OcrEngineMode.LstmOnly : OcrEngineMode.TesseractLstmCombined;
			_tesseract = new Tesseract(dataPath, lang, mode);
			_tesseract.SetVariable("user_defined_dpi", "300"); // Установка dpi, чтоб не ругался и не выдавал предупреждения
		}

		// Легкое распознавание 
		public async Task<string> Recognise(Bitmap src)
		{
			return await RecogniseHard(src);
		}

		// Тяжелое распознавание 
		public async Task<string> RecogniseHard(Bitmap src)
		{
			// TODO: использовать для распознавания цифр!!!
			// TODO: улучшить алгоритм распознавания цифр!!!
			return await Task.Run(() =>
			{
				string text = string.Empty;
				Image<Bgr, byte> image = src.ToImage<Bgr, byte>();
				_tesseract.SetImage(image);
				_tesseract.Recognize();
				text = _tesseract.GetUTF8Text();

				double coeff = 0d;
				double startGamma = 1.4d;
				double endGamma = 2.5d;
				double stepGamma = 0.2d;
				double gamma = startGamma;

				while (gamma <= endGamma && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
				{
					Image<Bgr, byte> imageCV = src.ToImage<Bgr, byte>(); // Получаем картинку из pictureBox1
					imageCV._GammaCorrect(gamma);
					_tesseract.SetImage(imageCV);
					_tesseract.Recognize();
					text = _tesseract.GetUTF8Text();

					coeff = 1.3d; // Чем меньше, тем больше захватывается соседних пикселей
					double endCoeff = 2.0d;
					double coeffStep = 0.349d;
					while (coeff <= endCoeff && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
					{
						//Изменение только в черные и белый(без оттенков)					
						for (int i = imageCV.Rows - 1; i >= 0; i--)
						{
							for (int j = imageCV.Cols - 1; j >= 0; j--)
							{
								if (imageCV.Data[i, j, 0] > byte.MaxValue / coeff)
								{
									imageCV.Data[i, j, 0] = byte.MaxValue;
									imageCV.Data[i, j, 1] = byte.MaxValue;
									imageCV.Data[i, j, 2] = byte.MaxValue;
								}
								else
								{
									imageCV.Data[i, j, 0] = 0;
									imageCV.Data[i, j, 1] = 0;
									imageCV.Data[i, j, 2] = 0;
								}
							}
						}
						_tesseract.SetImage(imageCV);
						_tesseract.Recognize();
						text = _tesseract.GetUTF8Text();
						coeff += coeffStep;
					}
					gamma += stepGamma;
				}
				return text;
			});
		}
	}
}
