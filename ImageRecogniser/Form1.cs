using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using ImageRecogniser.Models;

namespace ImageRecogniser
{

	public partial class Form1 : Form
	{
		// Для теста
		public List<Color> Colors { get; set; } = new List<Color>();

		private List<TestQuestion> TestQuestions = new List<TestQuestion>
		{
			new TestQuestion
			{
				Text = "Как звали отца няни А.С.Пушкина?",
				Variant1 = new Variant {Text = "Родион", IsRight = true},
				Variant2 = new Variant {Text = "Александр"},
				Variant3 = new Variant {Text = "Фёдор"},
				Variant4 = new Variant {Text = "Сергей"}
			},
			new TestQuestion
			{
				Text = "Какой остров расположен у восточного берега Африканского континента?",
				Variant1 = new Variant {Text = "Джерба"},
				Variant2 = new Variant {Text = "Сокотра"},
				Variant3 = new Variant {Text = "Мадагаскар", IsRight = true},
				Variant4 = new Variant {Text = "Цейлон"}
			},
			new TestQuestion
			{
				Text = "Кто был конструктором пистолета-пулемёта ППД?",
				Variant1 = new Variant {Text = "Евгений Драгунов"},
				Variant2 = new Variant {Text = "Генри Дерринджер"},
				Variant3 = new Variant {Text = "Иоганн Дрейзе"},
				Variant4 = new Variant {Text = "Василий Дегтярёв", IsRight = true}
			},
			new TestQuestion
			{
				Text = "Лучший бомбардир ФК \"Реал Мадрид\" за всю\r\nего историю на 2015 год:\r\n",
				Variant1 = new Variant {Text = "Ференц Пушкаш\r\n"},
				Variant2 = new Variant {Text = "Криштиану Роналду\r\n"},
				Variant3 = new Variant {Text = "Уго Санчес\r\n"},
				Variant4 = new Variant {Text = "Альфредо Ди Стефано\r\n", IsRight = true}
			}
		};
		private List<NumberQuestion> NumberQuestions = new List<NumberQuestion>
		{
			new NumberQuestion { Text = "В РФ срок действия патента на изобретение\r\nсоставляет ... лет:\r\n", Number = 20},
		};

		private int _screenshootNumber = -1;
		public int ScreenshootNumber
		{
			get
			{
				_screenshootNumber++;
				return _screenshootNumber;
			}
		}
		private ScreenResolution resolution = ScreenResolution.FullScreenForTest;
		private GameMode gameMode;

		private string path = string.Empty;
		private int width;
		private int height;

		public Bitmap CurrentScreen { get; set; }

		public Bitmap TextQuestion { get; private set; }
		public Bitmap Variant1 { get; private set; }
		public Bitmap Variant2 { get; private set; }
		public Bitmap Variant3 { get; private set; }
		public Bitmap Variant4 { get; private set; }

		public string QuestionText { get; private set; } = string.Empty;
		public string Variant1Text { get; private set; } = string.Empty;
		public string Variant2Text { get; private set; } = string.Empty;
		public string Variant3Text { get; private set; } = string.Empty;
		public string Variant4Text { get; private set; } = string.Empty;

		public Bitmap NumberQuestion { get; set; }
		public Bitmap NumberAnswer { get; set; }

		public string NumberQuestionText { get; set; } = string.Empty;
		public string NumberAnswerText { get; set; } = string.Empty;
		public bool IsFirstScreenFrame { get; private set; } = true;

		public Form1()
		{
			InitializeComponent();
		}

		private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult res = openFileDialog1.ShowDialog();
			if (res == DialogResult.OK)
			{
				path = openFileDialog1.FileName;
				Image img = Image.FromFile(path);
				CurrentScreen = new Bitmap(img);
				width = img.Width;
				height = img.Height;
				pictureBox1.Image = CurrentScreen;

				QuestionText = string.Empty;
				Variant1Text = string.Empty;
				Variant2Text = string.Empty;
				Variant3Text = string.Empty;
				Variant4Text = string.Empty;
			}
		}

		private string Recognise(Bitmap src)
		{
			var text = string.Empty;
			Tesseract tesseract = new Tesseract(@"C:\tessdata", "rusf", OcrEngineMode.LstmOnly);
			double coeff = 0d;
			double startGamma = 1.4d;
			double endGamma = 2.5d;
			double stepGamma = 0.2d;

			double gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
			{
				Image<Bgr, byte> imageCV = src.ToImage<Bgr, byte>(); // Получаем картинку из pictureBox1
				imageCV._GammaCorrect(gamma);
				tesseract.SetImage(imageCV);
				tesseract.Recognize();
				text = tesseract.GetUTF8Text();

				coeff = 1.3d; // Чем меньше, тем больше захватывается соседних пикселей
				double endCoeff = 2.0d;
				double coeffStep = 0.35d - 0.01d;
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
					tesseract.SetImage(imageCV);
					tesseract.Recognize();
					text = tesseract.GetUTF8Text();
					coeff += coeffStep;
				}
				gamma += stepGamma;
			}


			tesseract.Dispose();
			return text;
		}

		// Распознать (усложненная версия)
		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			var text = string.Empty;

			Tesseract tesseract = new Tesseract(@"C:\tessdata", "rusf", OcrEngineMode.LstmOnly);

			double coeff = 0d;
			double startGamma = 1.4d;
			double endGamma = 2.5d;
			double stepGamma = 0.2d;

			double gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
			{
				Image<Bgr, byte> imageCV = new Bitmap(pictureBox1.Image).ToImage<Bgr, byte>(); // Получаем картинку из pictureBox1
				imageCV._GammaCorrect(gamma);
				tesseract.SetImage(imageCV);
				tesseract.Recognize();
				text = tesseract.GetUTF8Text();

				coeff = 0.7d; // Чем меньше, тем больше захватывается соседних пикселей
				double endCoeff = 2.0d;
				double coeffStep = 0.35d - 0.01d;
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
					tesseract.SetImage(imageCV);
					tesseract.Recognize();
					text = tesseract.GetUTF8Text();
					coeff += coeffStep;
				}
				gamma += stepGamma;

				if (!(gamma <= endGamma && (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))))
				{
					pictureBox1.Image = imageCV.AsBitmap();
				}
			}



			richTextBox1.Text = text + "\n\n\nGamma: " + gamma + "\nCoeff: " + coeff;
		}

		// Распознать все (облегченная версия)
		private void recogniseAllBtn_Click(object sender, EventArgs e)
		{

			SetEmptyAllTexts();

			var timer = new Stopwatch();

			timer.Start();
			Tesseract tessaract = new Tesseract(@"C:\tessdata", "rusf", OcrEngineMode.LstmOnly);

			double startGamma = 1.4d;
			double endGamma = 2.0d;
			double gammaStep = 0.05d;

			double gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(QuestionText) || string.IsNullOrWhiteSpace(QuestionText)))
			{
				Image<Bgr, byte> imageCV = TextQuestion.ToImage<Bgr, byte>();
				imageCV._GammaCorrect(gamma);
				tessaract.SetImage(imageCV);
				tessaract.Recognize();
				QuestionText = tessaract.GetUTF8Text();
				gamma += gammaStep;
			}

			gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(Variant1Text) || string.IsNullOrWhiteSpace(Variant1Text)))
			{
				Image<Bgr, byte> imageCV = Variant1.ToImage<Bgr, byte>();
				imageCV._GammaCorrect(gamma);
				tessaract.SetImage(imageCV);
				tessaract.Recognize();
				Variant1Text = tessaract.GetUTF8Text();
				gamma += gammaStep;
			}

			gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(Variant2Text) || string.IsNullOrWhiteSpace(Variant2Text)))
			{
				Image<Bgr, byte> imageCV = Variant2.ToImage<Bgr, byte>();
				imageCV._GammaCorrect(gamma);
				tessaract.SetImage(imageCV);
				tessaract.Recognize();
				Variant2Text = tessaract.GetUTF8Text();
				gamma += gammaStep;
			}

			gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(Variant3Text) || string.IsNullOrWhiteSpace(Variant3Text)))
			{
				Image<Bgr, byte> imageCV = Variant3.ToImage<Bgr, byte>();
				imageCV._GammaCorrect(gamma);
				tessaract.SetImage(imageCV);
				tessaract.Recognize();
				Variant3Text = tessaract.GetUTF8Text();
				gamma += gammaStep;
			}


			gamma = startGamma;
			while (gamma <= endGamma && (string.IsNullOrEmpty(Variant4Text) || string.IsNullOrWhiteSpace(Variant4Text)))
			{
				Image<Bgr, byte> imageCV = Variant4.ToImage<Bgr, byte>();
				imageCV._GammaCorrect(gamma);
				tessaract.SetImage(imageCV);
				tessaract.Recognize();
				Variant4Text = tessaract.GetUTF8Text();
				gamma += gammaStep;
			}
			timer.Stop();

			tessaract.Dispose();
			richTextBox1.Text = QuestionText + "\n\n" + "1) " + Variant1Text + "\n2) " + Variant2Text + "\n3) " + Variant3Text + "\n4) " + Variant4Text + "\n\nВремя: " + timer.Elapsed.TotalSeconds + " с.";
		}

		private void SetEmptyAllTexts()
		{
			QuestionText = string.Empty;
			Variant1Text = string.Empty;
			Variant2Text = string.Empty;
			Variant3Text = string.Empty;
			Variant4Text = string.Empty;

			NumberQuestionText = string.Empty;
			NumberAnswerText = string.Empty;
		}

		// Нарезать 
		private void cutToolStripButton2_Click(object sender, EventArgs e)
		{
			if (gameMode == GameMode.TestQuestion)
			{
				int questionX;
				int questionY;
				int questionWidth;
				int questionHeigth;

				int firstRectLeftTopCoordX;
				int firstRectLeftTopCoordY;
				int between;
				int varWidth;
				int varHeigth;

				InitializeValues(resolution, out questionX, out questionY, out questionWidth, out questionHeigth, out firstRectLeftTopCoordX, out firstRectLeftTopCoordY, out between, out varWidth, out varHeigth);

				Rectangle rect1 = new Rectangle(new Point(questionX, questionY), new Size(questionWidth, questionHeigth)); // Вопрос
				Rectangle rect2 = new Rectangle(new Point(firstRectLeftTopCoordX, firstRectLeftTopCoordY), new Size(varWidth, varHeigth)); // Вариант 1
				Rectangle rect3 = new Rectangle(new Point(firstRectLeftTopCoordX, firstRectLeftTopCoordY + 1 * varHeigth + 1 * between), new Size(varWidth, varHeigth)); // Вариант 2
				Rectangle rect4 = new Rectangle(new Point(firstRectLeftTopCoordX, firstRectLeftTopCoordY + 2 * varHeigth + 2 * between), new Size(varWidth, varHeigth)); // Вариант 3
				Rectangle rect5 = new Rectangle(new Point(firstRectLeftTopCoordX, firstRectLeftTopCoordY + 3 * varHeigth + 3 * between), new Size(varWidth, varHeigth)); // Вариант 4

				TextQuestion = CutImage(CurrentScreen, rect1);
				Variant1 = CutImage(CurrentScreen, rect2);
				Variant2 = CutImage(CurrentScreen, rect3);
				Variant3 = CutImage(CurrentScreen, rect4);
				Variant4 = CutImage(CurrentScreen, rect5);

				pictureBox1.Image = TextQuestion;
			}
			else if (gameMode == GameMode.NumericQuestion)
			{
				CutForNumberQuestion();
				pictureBox1.Image = NumberQuestion;
			}
		}

		private void CutForNumberQuestion()
		{
			Rectangle rect1 = new Rectangle(new Point(506, 365), new Size(575, 109)); // Вопрос
			Rectangle rect2 = new Rectangle(new Point(678, 485), new Size(246, 60)); // Ответ

			NumberQuestion = CutImage(CurrentScreen, rect1);
			NumberAnswer = CutImage(CurrentScreen, rect2);
		}

		private void InitializeValues(ScreenResolution resolution, out int questionX, out int questionY, out int questionWidth, out int questionHeigth, out int firstRectLeftTopCoordX, out int firstRectLeftTopCoordY, out int between, out int varWidth, out int varHeigth)
		{
			if (resolution == ScreenResolution.FullScreenForTest)
			{
				questionX = 491;
				questionY = 364;
				questionWidth = 604;
				questionHeigth = 118;

				firstRectLeftTopCoordX = 575;
				firstRectLeftTopCoordY = 507;
				between = 20;
				varWidth = 443;
				varHeigth = 62;
			}
			else if (resolution == ScreenResolution.HalfScreen)
			{
				questionX = 76;
				questionY = 388;
				questionWidth = 501;
				questionHeigth = 110;

				firstRectLeftTopCoordX = 149;
				firstRectLeftTopCoordY = 512;
				between = 17;
				varWidth = 360;
				varHeigth = 50;
			}
			else
			{
				throw new Exception();
			}
		}

		private Bitmap CutImage(Bitmap src, Rectangle rect)
		{
			Bitmap bmp = new Bitmap(src.Width, src.Height);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(src, 0, 0, rect, GraphicsUnit.Pixel);
			return bmp;
		}

		private void questionBtn_Click(object sender, EventArgs e)
		{
			if (gameMode == GameMode.TestQuestion)
			{
				pictureBox1.Image = TextQuestion;
			}
			else if (gameMode == GameMode.NumericQuestion)
			{
				pictureBox1.Image = NumberQuestion;
			}
		}

		private void variant1Btn_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = Variant1;
		}

		private void variant2Btn_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = Variant2;
		}

		private void variant3Btn_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = Variant3;
		}

		private void variant4Btn_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = Variant4;
		}

		private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox1.SelectedIndex == 0)
			{
				gameMode = GameMode.TestQuestion;
			}
			else if (toolStripComboBox1.SelectedIndex == 1)
			{
				gameMode = GameMode.NumericQuestion;
			}
		}

		private void ScreenShoot()
		{
			if (!int.TryParse(processIdTextBox.Text, out int processId))
			{
				MessageBox.Show("Неверный ID процесса");
				return;
			}
			var process = Process.GetProcesses().Where(p => p.Id == processId).FirstOrDefault();
			if (process == null)
			{
				MessageBox.Show("Неверный ID процесса");
				return;
			}
			// проверка 
			var hwnd = process.MainWindowHandle;
			if (hwnd.ToInt32() == 0x00000000)
			{
				MessageBox.Show("Неверный ID процесса");
				return;
			}
			GetWindowRect(hwnd, out RECT rect);
			using (Bitmap image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top))
			{
				using (var graphics = Graphics.FromImage(image))
				{
					var hdcBitmap = graphics.GetHdc();
					PrintWindow(hwnd, hdcBitmap, 0);
					graphics.ReleaseHdc(hdcBitmap);
				}
				CurrentScreen = image.ToImage<Bgr, byte>().AsBitmap();
				pictureBox1.Image = CurrentScreen;
				IsFirstScreenFrame = false;
				CurrentScreen.Save($"screenshoots\\new_cadr_{ScreenshootNumber}.png", ImageFormat.Png);
				GC.Collect();
			}
		}


		[DllImport("user32.dll", SetLastError = true)]
		static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll")]
		static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left, Top, Right, Bottom;
		}

		private void screenshootBtn_Click(object sender, EventArgs e)
		{
			ScreenShoot();
		}

		private void processIdTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			{

				if (char.IsDigit(e.KeyChar) || (int)e.KeyChar == 8)
					return;
				else
					e.Handled = true;
			}
		}

		private async Task AnalyzeFrame(Bitmap src)
		{
			var firstPixel = src.GetPixel(1190, 468); // RGB (255, 255, 204) сверху справа
			var secondPixel = src.GetPixel(509, 517); // RGB (242, 233, 219) сверху слева
			var thirdPixel = src.GetPixel(1184, 816); // RGB (249, 245, 238) снизу справа
			var fourthPixel = src.GetPixel(947, 758); // RGB на активном вопросе (255, 255, 255) /  на зеленой кнопке (0, 213, 0) / на оценке вопроса (238, 224, 157)
			var fivethPixel = src.GetPixel(549, 564); // RGB (140, 74, 31) на краю дартца (после ответа на циферный вопрос) - показ результатов

			if (firstPixel.R == 255 && firstPixel.G == 255 && firstPixel.B == 204
				&& secondPixel.R == 242 && secondPixel.G == 233 && secondPixel.B == 219
				&& thirdPixel.R == 249 && thirdPixel.G == 245 && thirdPixel.B == 238)
			{
				// На экране вопрос (какой? еще неизвестно)

				if (fourthPixel.R == 255 && fourthPixel.G == 255 && fourthPixel.B == 255)
				{
					// На экране АКТИВНЫЙ тестовый вопрос
					cutToolStripButton2_Click(new object(), new EventArgs()); // Нарезка тестового вопроса
					recogniseAllBtn_Click(new object(), new EventArgs()); // Распознавание тестового вопроса

					// Поиск в БД
					var question = TestQuestions.FirstOrDefault(q => q.Text == QuestionText);
					if (question == null) // Вопрос новый
					{
						// Добавляем новый в бд
						var newQuest = new TestQuestion
						{
							Text = QuestionText,
							Variant1 = new Variant { Text = Variant1Text },
							Variant2 = new Variant { Text = Variant2Text },
							Variant3 = new Variant { Text = Variant3Text },
							Variant4 = new Variant { Text = Variant4Text }
						};
						TestQuestions.Add(newQuest);
						// Отвечаем на рандом 
						Random rnd = new Random();
						int answerNumber = rnd.Next(1, 4 + 1); // От 1 до 4

						// Клик по рандомному TODO
						MessageBox.Show("Здесь кликаем по " + answerNumber);

						// Ждем, когда будет ответ 
						bool isAnswerFounded = false;
						int rightAnswerNumber = 0;
						int count = 0; // Количество проходов цикла
						while (!isAnswerFounded && count < 30)
						{
							ScreenShoot(); // Получение нового скриншота в CurrentScreen
							var firstAnswerPixel = CurrentScreen.GetPixel(670, 511); // RGB (255, 220, 127) - желтый с правильным ответом
							var secondAnswerPixel = CurrentScreen.GetPixel(670, 594);
							var thirdAnswerPixel = CurrentScreen.GetPixel(670, 675);
							var fourthAnswerPixel = CurrentScreen.GetPixel(670, 757);

							// Для тестов
							Colors.AddRange(new Color[] { firstAnswerPixel, secondAnswerPixel, thirdAnswerPixel, fourthAnswerPixel });

							// Проверка на желтый (если да, то найден ответ)
							if (firstAnswerPixel.R > 250 && firstAnswerPixel.R <= 255 && firstAnswerPixel.G > 185 && firstAnswerPixel.G < 225 && firstAnswerPixel.B >= 0 && firstAnswerPixel.B < 220)
							{
								rightAnswerNumber = 1;
								isAnswerFounded = true;
								break;
							}
							else if (secondAnswerPixel.R > 250 && secondAnswerPixel.R <= 255 && secondAnswerPixel.G > 185 && secondAnswerPixel.G < 225 && secondAnswerPixel.B >= 0 && secondAnswerPixel.B < 220)
							{
								rightAnswerNumber = 2;
								isAnswerFounded = true;
								break;
							}
							else if (thirdAnswerPixel.R > 250 && thirdAnswerPixel.R <= 255 && thirdAnswerPixel.G > 185 && thirdAnswerPixel.G < 225 && thirdAnswerPixel.B >= 0 && thirdAnswerPixel.B < 220)
							{
								rightAnswerNumber = 3;
								isAnswerFounded = true;
								break;
							}
							else if (fourthAnswerPixel.R > 250 && fourthAnswerPixel.R <= 255 && fourthAnswerPixel.G > 185 && fourthAnswerPixel.G < 225 && fourthAnswerPixel.B >= 0 && fourthAnswerPixel.B < 220)
							{
								rightAnswerNumber = 4;
								isAnswerFounded = true;
								break;
							}

							count++;
							await Task.Delay(100); // Задержка 
						}


						if (isAnswerFounded) // Найден ответ - сохраняем
						{
							// Сохраняем правильный ответ
							if (rightAnswerNumber == 1)
							{
								newQuest.Variant1.SetRight();
							}
							else if (rightAnswerNumber == 2)
							{
								newQuest.Variant2.SetRight();
							}
							else if (rightAnswerNumber == 3)
							{
								newQuest.Variant3.SetRight();
							}
							else if (rightAnswerNumber == 4)
							{
								newQuest.Variant4.SetRight();
							}

							MessageBox.Show("Записали правильный ответ - " + rightAnswerNumber);
						}
						else // Не нашли ответ, проглядели - удаляем запись без ответа из БД
						{
							TestQuestions.Remove(newQuest);
							MessageBox.Show("Не удалось отследить ответ");
						}

					}
					else // Если ответ найден в БД
					{
						string rightText = question.GetRightOrDefault()?.Text;
						int rightNumber = 0;
						if (rightText != null) // Получаем правильный номер вопроса
						{
							if (Variant1Text == rightText)
							{
								rightNumber = 1;

							}
							else if (Variant2Text == rightText)
							{
								rightNumber = 2;
							}
							else if (Variant3Text == rightText)
							{
								rightNumber = 3;
							}
							else if (Variant4Text == rightText)
							{
								rightNumber = 4;
							}
						}

						// Клик по правильному TODO

						MessageBox.Show("Кликаем по номеру " + rightNumber + " с текстом: " + rightText);
					}
				}
				else if (fourthPixel.R == 238 && fourthPixel.G == 224 && fourthPixel.B == 157)
				{
					// На экране НЕАКТИВНЫЙ тестовый вопрос
					cutToolStripButton2_Click(new object(), new EventArgs()); // Нарезка тестового вопроса
					recogniseAllBtn_Click(new object(), new EventArgs()); // Распознавание тестового вопроса

					// Поиск в БД
					var question = TestQuestions.FirstOrDefault(q => q.Text == QuestionText);
					if (question == null) // Вопрос новый
					{
						// Добавляем новый в бд
						var newQuest = new TestQuestion
						{
							Text = QuestionText,
							Variant1 = new Variant { Text = Variant1Text },
							Variant2 = new Variant { Text = Variant2Text },
							Variant3 = new Variant { Text = Variant3Text },
							Variant4 = new Variant { Text = Variant4Text }
						};
						TestQuestions.Add(newQuest);

						// Ждем, когда будет ответ 
						bool isAnswerFounded = false;
						int rightAnswerNumber = 0;
						int count = 0; // Количество проходов цикла.
						while (!isAnswerFounded && count < 30)
						{
							ScreenShoot(); // Получение нового скриншота в CurrentScreen
							var firstAnswerPixel = CurrentScreen.GetPixel(670, 511); // RGB (255, 220, 127) - желтый с правильным ответом
							var secondAnswerPixel = CurrentScreen.GetPixel(670, 594);
							var thirdAnswerPixel = CurrentScreen.GetPixel(670, 675);
							var fourthAnswerPixel = CurrentScreen.GetPixel(670, 757);

							// Проверка на желтый (если да, то найден ответ)
							if (firstAnswerPixel.R > 250 && firstAnswerPixel.R <= 255 && firstAnswerPixel.G > 185 && firstAnswerPixel.G < 225 && firstAnswerPixel.B >= 0 && firstAnswerPixel.B < 220)
							{
								rightAnswerNumber = 1;
								isAnswerFounded = true;
								break;
							}
							else if (secondAnswerPixel.R > 250 && secondAnswerPixel.R <= 255 && secondAnswerPixel.G > 185 && secondAnswerPixel.G < 225 && secondAnswerPixel.B >= 0 && secondAnswerPixel.B < 220)
							{
								rightAnswerNumber = 2;
								isAnswerFounded = true;
								break;
							}
							else if (thirdAnswerPixel.R > 250 && thirdAnswerPixel.R <= 255 && thirdAnswerPixel.G > 185 && thirdAnswerPixel.G < 225 && thirdAnswerPixel.B >= 0 && thirdAnswerPixel.B < 220)
							{
								rightAnswerNumber = 3;
								isAnswerFounded = true;
								break;
							}
							else if (fourthAnswerPixel.R > 250 && fourthAnswerPixel.R <= 255 && fourthAnswerPixel.G > 185 && fourthAnswerPixel.G < 225 && fourthAnswerPixel.B >= 0 && fourthAnswerPixel.B < 220)
							{
								rightAnswerNumber = 4;
								isAnswerFounded = true;
								break;
							}

							count++;
							await Task.Delay(100); // Задержка 
						}

						if (isAnswerFounded) // Нашли ответ - сохраняем
						{
							// Сохраняем правильный ответ
							if (rightAnswerNumber == 1)
							{
								newQuest.Variant1.SetRight();
							}
							else if (rightAnswerNumber == 2)
							{
								newQuest.Variant2.SetRight();
							}
							else if (rightAnswerNumber == 3)
							{
								newQuest.Variant3.SetRight();
							}
							else if (rightAnswerNumber == 4)
							{
								newQuest.Variant4.SetRight();
							}
							MessageBox.Show("Записали правильный ответ - " + rightAnswerNumber);
						}
						else // Не нашли ответ, проглядели - удаляем запись без ответа из БД
						{
							TestQuestions.Remove(newQuest);
							MessageBox.Show("Не удалось отследить ответ");
						}

					}
					else // Если уже имеем такой вопрос в бд, то нет смысла и следить 
					{
						MessageBox.Show("Уже имеем такой вопрос");
						return;
					}
				}
				else if (fivethPixel.R == 140 && fivethPixel.G == 74 && fivethPixel.B == 31)
				{
					// На экране ОТВЕТ вопроса с цифрами

					// Пытаемся распознать
					CutForNumberQuestion(); // Разрезать
					NumberQuestionText = Recognise(NumberQuestion); // Распознать
					NumberAnswerText = Recognise(NumberAnswer).Trim('\r', '\n', ' '); // Распознать

					// Если не пусто в ответе
					if (!(string.IsNullOrEmpty(NumberAnswerText) || string.IsNullOrWhiteSpace(NumberAnswerText)))
					{
						var question = NumberQuestions.FirstOrDefault(q => q.Text == NumberQuestionText);
						if (question == null) // В базе пусто - добавляем
						{
							if (!(string.IsNullOrEmpty(NumberQuestionText) || string.IsNullOrWhiteSpace(NumberQuestionText)) && int.TryParse(NumberAnswerText, out int n))
							{
								// Если не пустой вопрос и удался парсинг строки
								NumberQuestions.Add(new NumberQuestion
								{
									Text = NumberQuestionText,
									Number = n // Напарсенная цифра
								});

								MessageBox.Show("Добавлен новый цифровой ответ. Текст: " + NumberQuestionText + "\nЦифра: " + NumberAnswerText);
							}
						}
					}
					else // В ответе пусто - не можем написать
					{
						MessageBox.Show("Цифра в ответе не распозналась. Запись не создана");
					}
				}
				else if (fourthPixel.R == 0 && fourthPixel.G == 213 && fourthPixel.B == 0)
				{
					// На экране АКТИВНЫЙ вопрос в цифрами

					// Пытаемся распознать
					CutForNumberQuestion(); // Разрезать
					NumberQuestionText = Recognise(NumberQuestion); // Распознать

					var question = NumberQuestions.FirstOrDefault(q => q.Text == NumberQuestionText);
					if (question == null) // В базе пусто - рандомно отвечаем
					{
						if (NumberQuestionText.ToLower().Contains("век"))
						{
							// TODO набрать 18
							MessageBox.Show("Пишем 18");
						}
						else if (NumberQuestionText.ToLower().Contains("год"))
						{
							// TODO набрать 1969
							MessageBox.Show("Пишем 1969");
						}
						else
						{
							// TODO набрать 5
							MessageBox.Show("Пишем 5");
						}
					}
					else // Если есть такой вопрос
					{
						// TODO набрать n
						MessageBox.Show("Пишем " + question.Number);
					}
				}

			}
		}

		private async void analyzeBtn_Click(object sender, EventArgs e)
		{
			while (true)
			{
				ScreenShoot();
				await AnalyzeFrame(CurrentScreen);
				await Task.Delay(1000);
			}
		}

		private void numericAnswerBtn_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = NumberAnswer;
		}


		private async void testBtn_Click_1(object sender, EventArgs e)
		{

		}
	}

	public enum ScreenResolution
	{
		FullScreenForTest,
		HalfScreen
	}

	public enum GameMode
	{
		TestQuestion,
		NumericQuestion
	}
}



