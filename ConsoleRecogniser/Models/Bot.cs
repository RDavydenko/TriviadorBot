using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class Bot
	{
		private int _counter = 0; // Счетчик смены вопроса, каждые n раз - сохранение репозиториев		
		private readonly Process _process;

		private readonly SyncMode mode; // Указывает на то, какой используется распознаватель (синхронный/асинхронный)
		private readonly IRecogniser _recogniser;
		private readonly IRecogniserAsync _recogniserAsync; // Опционально

		private readonly IRepository<TestQuestion> _testRepository;
		private readonly IRepository<NumericQuestion> _numericRepository;
		private readonly ICutter _cutter;

		public Bitmap Screen { get; private set; }
		public GameSituation Status { get; private set; } = GameSituation.Default;

		public string TestText { get; private set; } // Текст ТЕКУЩЕГО текстового вопроса
		public int RightTestNumber { get; private set; }  // Текущий правильный в тестовом задании ответ (который подсветился)

		public string NumericText { get; private set; } // Текст ТЕКУЩЕГО циферного вопроса	


		public Bot(int processId, IRecogniser recogniser, IRepository<TestQuestion> testRepository, IRepository<NumericQuestion> numberRepository, ICutter cutter)
		{
			_process = Process.GetProcesses().FirstOrDefault(p => p.Id == processId);
			if (_process == null)
			{
				Console.WriteLine("Процесс не найден");
				throw new Exception();
			}
			_recogniser = recogniser;
			mode = SyncMode.Sync; // Синхронный распознаватель

			_testRepository = testRepository;
			_numericRepository = numberRepository;
			_cutter = cutter;
		}

		public Bot(int processId, IRecogniserAsync recogniserAsync, IRepository<TestQuestion> testRepository, IRepository<NumericQuestion> numberRepository, ICutter cutter)
		{
			_process = Process.GetProcesses().FirstOrDefault(p => p.Id == processId);
			if (_process == null)
			{
				Console.WriteLine("Процесс не найден");
				throw new Exception();
			}
			_recogniserAsync = recogniserAsync;
			mode = SyncMode.Async; // Ассинхронный распознаватель

			_testRepository = testRepository;
			_numericRepository = numberRepository;
			_cutter = cutter;
		}

		private GameSituation Analyze(Bitmap src)
		{
			var firstPixel = src.GetPixel(1190, 468); // RGB (255, 255, 204) сверху справа
			var secondPixel = src.GetPixel(509, 517); // RGB (242, 233, 219) сверху слева
			var thirdPixel = src.GetPixel(1184, 816); // RGB (249, 245, 238) снизу справа
			var fourthPixel = src.GetPixel(998, 619); // RGB на тестовом вопросе от (255, 255, 255) до (250, 250, 250) - белый
			var fivethPixel = src.GetPixel(543, 643); // RGB (165, 88, 37) на краю дартца, если вопрос циферный

			bool isQuestion = firstPixel.R == 255 && firstPixel.G == 255 && firstPixel.B == 204
							&& secondPixel.R == 242 && secondPixel.G == 233 && secondPixel.B == 219
							&& thirdPixel.R == 249 && thirdPixel.G == 245 && thirdPixel.B == 238;

			if (isQuestion) // На экране вопрос (какой? еще неизвестно)
			{
				bool isTest = !(fivethPixel.R == 165 && fivethPixel.G == 88 && fivethPixel.B == 37); // Вопрос тестовый (не циферный)
				var sixthPixel = src.GetPixel(1028, 735); // RGB на полоске оценки вопроса, слегка желтая (238, 224, 157) И на тестовом, и на цифровом
				bool isInactive = sixthPixel.R == 238 && sixthPixel.G == 224 && sixthPixel.B == 157;

				if (isTest) // На экране тестовый вопрос
				{
					var seventhPixel = src.GetPixel(970, 510); // RGB на правильном ответе 1
					var eighthPixel = src.GetPixel(971, 592); // RGB на правильном ответе 2
					var ninethPixel = src.GetPixel(972, 675); // RGB на правильном ответе 3
					var tenthPixel = src.GetPixel(867, 814); // RGB на правильном ответе 4
					bool isFirstRight = seventhPixel.R > 220 && seventhPixel.R <= 255 && seventhPixel.G > 160 && seventhPixel.G < 225 && seventhPixel.B >= 0 && seventhPixel.B < 220;
					bool isSecondRight = eighthPixel.R > 220 && eighthPixel.R <= 255 && eighthPixel.G > 160 && eighthPixel.G < 225 && eighthPixel.B >= 0 && eighthPixel.B < 220;
					bool isThirdRight = ninethPixel.R > 220 && ninethPixel.R <= 255 && ninethPixel.G > 160 && ninethPixel.G < 225 && ninethPixel.B >= 0 && ninethPixel.B < 220;
					bool isFourthRight = tenthPixel.R > 220 && tenthPixel.R <= 255 && tenthPixel.G > 160 && tenthPixel.G < 225 && tenthPixel.B >= 0 && tenthPixel.B < 220;

					// Если какой-то из вариантов мигает, что он верный
					if (isFirstRight)
					{
						RightTestNumber = 1;
						return GameSituation.AnswerTest;
					}
					else if (isSecondRight)
					{
						RightTestNumber = 2;
						return GameSituation.AnswerTest;
					}
					else if (isThirdRight)
					{
						RightTestNumber = 3;
						return GameSituation.AnswerTest;
					}
					else if (isFourthRight)
					{
						RightTestNumber = 4;
						return GameSituation.AnswerTest;
					}

					if (isInactive) // Тестовый неактивный (и без ответа)
					{
						if (Status == GameSituation.AnswerTest) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Простой
						{
							return GameSituation.Idle;
						}
						return GameSituation.InactiveTest;
					}

					if (Status == GameSituation.AnswerTest) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Простой
					{
						return GameSituation.Idle;
					}
					return GameSituation.ActiveTest;
				}

				bool isNumeric = !isTest; // Циферный вопрос
				if (isNumeric) // Циферный вопрос
				{
					var eleventhPixel = src.GetPixel(903, 497); // На белом поле ответа RGB (0, 213, 0)
					var twelvethPixel = src.GetPixel(945, 686); // На зеленой кнопке, если активный RGB (0, 213, 0)
					bool IsInAnswerProcess = twelvethPixel.R == 0 && twelvethPixel.G == 213 && twelvethPixel.B == 0; // В процессе ответа
					if (IsInAnswerProcess)
					{
						return GameSituation.ActiveNumeric; // В процессе отвечания = активный без ответа
					}

					bool IsAnswered = eleventhPixel.R > 250 && eleventhPixel.G > 250 && eleventhPixel.B > 250; // Белый цвет = поле для ответа видно

					if (IsAnswered)
					{
						return GameSituation.AnswerNumeric; // Ответ на циферный вопрос виден
					}

					if (isInactive)
					{
						if (Status == GameSituation.AnswerNumeric) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Простой
						{
							return GameSituation.Idle;
						}
						return GameSituation.InactiveNumeric; // Неактивный циферный и без ответа
					}

					if (Status == GameSituation.AnswerNumeric) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Простой
					{
						return GameSituation.Idle;
					}
					return GameSituation.AnswerNumeric; // Активный циферный и без ответа
				}
			}
			return GameSituation.Idle;
		}

		private void Say(GameSituation status)
		{
			switch (status)
			{
				case GameSituation.Idle:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case GameSituation.ActiveTest:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case GameSituation.InactiveTest:
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case GameSituation.AnswerTest:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case GameSituation.ActiveNumeric:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case GameSituation.InactiveNumeric:
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case GameSituation.AnswerNumeric:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
			}
			Console.WriteLine(status);
			Console.ResetColor();
		}

		private async Task Act(Bitmap screen, GameSituation status)
		{
			if (status == GameSituation.Idle)
			{
				return;
			}

			if (status == GameSituation.ActiveTest || status == GameSituation.InactiveTest)
			{
				var questionTextBitmap = _cutter.CutQuestionText(Screen);
				string questionText = string.Empty;
				if (mode == SyncMode.Sync)
					questionText = _recogniser.Recognise(questionTextBitmap);
				else if (mode == SyncMode.Async)
					questionText = await _recogniserAsync.Recognise(questionTextBitmap);

				if (!string.IsNullOrEmpty(questionText) && !string.IsNullOrWhiteSpace(questionText))
				{
					if (questionText == TestText) // Если ничего не поменялось, то выходим, т.к. вопрос еще не успел смениться
					{
						Console.WriteLine("Уже следим за этим вопросом -_-");
						return;
					}

					TestText = questionText; // Запоминаем текущий
					var question = _testRepository.FirstOrDefault(questionText); // Ищем в БД
					if (question == null) // Вопрос новый 
					{
						// Создаем новую запись о тестовом вопросе						
						var newQuestion = new TestQuestion
						{
							Text = questionText
						};
						_testRepository.Add(newQuestion); // Добавляем новую запись в БД

						if (status == GameSituation.ActiveTest) // Время отвечать 
						{
							Console.WriteLine("Отвечай на рандом!");
						}
						return;
					}
					else // Вопрос уже есть в БД
					{
						if (status == GameSituation.ActiveTest) // Время отвечать
						{
							// Получаем правильный ответ из БД
							var rigthVar = question.GetRightOrDefault();
							if (rigthVar == null) // Такого не должно быть, но вдруг
							{
								Console.WriteLine("Не смог найти ответ, однако в БД вопрос уже есть :(");
							}
							else
							{
								int rightNumber = 0;
								string rightText = string.Empty;
								for (int i = 1; i <= 4; i++) // Пройдемся по всем вариантам, найдем с соответствующим текстом
								{
									string text = string.Empty;
									if (mode == SyncMode.Sync)
										text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, i));
									else if (mode == SyncMode.Async)
										text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, i));

									if (question.Answer == text) // Правильный варинат в текущей расстановке ответов найден!
									{
										rightText = text;
										rightNumber = i;
										break;
									}
								}
								Console.WriteLine($"Ответ найден! Кликай по {rightNumber} номеру с текстом: {rightText}");
							}
						}
						return;
					}
				}
			}

			if (status == GameSituation.AnswerTest)
			{
				if (TestText == string.Empty) // Пропуск, т.к. нет (не)активного вопроса в данный момент
				{
					return;
				}

				var question = _testRepository.FirstOrDefault(TestText);
				if (question == null) // Не должно быть такого, но если перед AnswerTest не было ActiveTest или InactiveTest
				{
					Console.WriteLine("Не записали ответ. Теперь в базе дырка :(");
					return;
				}
				if (question.HasRight()) // Если уже есть ответ на этот вопрос
				{
					Console.WriteLine("Ответ на этот вопрос уже есть в базе. Пропускаем");
					return;
				}

				string rightText = string.Empty; // Получаем текст правильного ответа
				if (mode == SyncMode.Sync)
					rightText = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, RightTestNumber));
				else if (mode == SyncMode.Async)
					rightText = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, RightTestNumber));

				if (string.IsNullOrEmpty(rightText) || string.IsNullOrWhiteSpace(rightText)) // Пустой ответ - ошибка распознавания
				{
					Console.WriteLine("Не получилось распознать правильный вариант :(");
					return;
				}
				question.SetRight(rightText); // Устанавливаем правильный текст
				_counter++; // Увеличиваем счетчик записанный ответов для последующего сохранения
				Console.WriteLine("Правильный вариант сохранен ^_^");
				TestText = string.Empty; // Убираем текст текущего вопроса, чтобы больше не пытаться искать и отслеживать ответ
				return;
			}

			if (status == GameSituation.ActiveNumeric || status == GameSituation.InactiveNumeric)
			{
				string numericText = string.Empty;
				if (mode == SyncMode.Sync)
					numericText = _recogniser.Recognise(_cutter.CutNumericText(Screen));
				else if (mode == SyncMode.Async)
					numericText = await _recogniserAsync.Recognise(_cutter.CutNumericText(Screen));

				if (!string.IsNullOrEmpty(numericText) && !string.IsNullOrWhiteSpace(numericText))
				{
					if (NumericText == numericText) // Если ничего не поменялось, то выходим, т.к. вопрос еще не успел смениться
					{
						Console.WriteLine("Уже следим за этим вопросом -_-");
						return;
					}
					NumericText = numericText; // Запоминаем текущий
					var question = _numericRepository.FirstOrDefault(numericText); // Ищем в БД
					if (question == null) // Вопрос новый 
					{
						// Создаем новую запись о тестовом вопросе						
						var newQuestion = new NumericQuestion
						{
							Text = numericText
						};
						_numericRepository.Add(newQuestion); // Добавляем новую запись в БД

						if (status == GameSituation.ActiveNumeric) // Время отвечать 
						{
							int randomNumber = 5; // Случайная цифра
												  // Псевдорандом по вхождению строк
							if (numericText.Contains("век") && !numericText.Contains("человек"))
							{
								randomNumber = 18;
							}
							else if (numericText.Contains("год"))
							{
								randomNumber = 1969;
							}

							Console.WriteLine($"Отвечай на рандом! Например {randomNumber}");
						}
						return;
					}
					else // Вопрос уже есть в БД
					{
						if (status == GameSituation.ActiveNumeric) // Время отвечать
						{
							// Получаем правильный ответ из БД
							int? rightAnswer = question.GetRightOrDefault();
							if (rightAnswer == null) // Такого не должно быть, но вдруг, если цифра не распозналась
							{
								Console.WriteLine("Не смог найти ответ, однако в БД вопрос уже есть :(");
								int randomNumber = 5; // Случайная цифра
													  // Псевдорандом по вхождению строк
								if (numericText.Contains("век") && !numericText.Contains("человек"))
								{
									randomNumber = 18;
								}
								else if (numericText.Contains("год"))
								{
									randomNumber = 1969;
								}

								Console.WriteLine($"Отвечай на рандом! Например {randomNumber}");
							}
							else
							{
								Console.WriteLine($"Ответ найден! Набирай {rightAnswer}");
							}
						}
						return;
					}
				}
			}

			if (status == GameSituation.AnswerNumeric)
			{
				if (NumericText == string.Empty) // Пропуск, т.к. нет (не)активного вопроса в данный момент
				{
					return;
				}

				var question = _numericRepository.FirstOrDefault(NumericText);
				if (question == null) // Не должно быть такого, но если перед AnswerTest не было ActiveTest или InactiveTest
				{
					Console.WriteLine("Не успели записали текст вопроса :(");
					return;
				}
				if (question.HasRight()) // Если уже есть ответ на этот вопрос
				{
					Console.WriteLine("Ответ на этот вопрос уже есть в базе. Пропускаем");
					return;
				}

				string rightText = string.Empty; // Получаем текст правильного ответа
				if (mode == SyncMode.Sync)
					rightText = _recogniser.RecogniseHard(_cutter.CutNumericAnswer(Screen)).Trim('\r', '\n', '\t', ' ', ' ');
				else if (mode == SyncMode.Async)
					rightText = (await _recogniserAsync.RecogniseHard(_cutter.CutNumericAnswer(Screen))).Trim('\r', '\n', '\t', ' ', ' ');

				bool isParsed = int.TryParse(rightText, out int answerNumber); // Пробуем парсить
				if (string.IsNullOrEmpty(rightText) || string.IsNullOrWhiteSpace(rightText)) // Пустой ответ - ошибка распознавания
				{
					Console.WriteLine("Не получилось распознать правильный вариант :(");
					return;
				}
				if (!isParsed)
				{
					Console.WriteLine($"Не получилось получить целое число из строки {{{rightText}}}");
				}
				question.SetRight(answerNumber); // Устанавливаем правильный ответ (число)
				_counter++; // Увеличиваем счетчик записанный ответов для последующего сохранения
				Console.WriteLine("Правильный вариант сохранен ^_^");
				NumericText = string.Empty; // Убираем текст текущего вопроса, чтобы больше не пытаться искать и отслеживать ответ
				return;
			}
		}

		private void SaveRepositories()
		{
			int n = 2; // Каждые n добавленных вопросов происходит сохранение репоситориев. 2 - каждый раз
			if (_counter % n == 0 && _counter != 0)
			{
				_testRepository.Save();
				_numericRepository.Save();
				_counter++;
			}
		}

		public async Task StartAsync()
		{
			while (true)
			{
				var sw = new Stopwatch();
				Screen = _process.Screenshoot(); // Получение нового скрина приложения
				var status = Analyze(Screen); // Анализ нового скрина
				if (Status != status) // Если статус сменился
				{
					Say(status); // Описание статуса анализа 
				}
				Status = status;

				sw.Start();
				await Act(Screen, Status); // Действие на основе скрина и статуса
				sw.Stop();
				if (Status != GameSituation.Idle)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("[Время обработки: ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write($"{ sw.Elapsed.TotalSeconds.ToString("0.000")}");
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write(" сек.]\n");
					Console.ResetColor();
				}

				SaveRepositories(); // Сохранение репозиториев

				await Task.Delay(200); // Задержка смены кадра
			}
		}


		private enum SyncMode
		{
			Sync,
			Async
		}
	}
}
