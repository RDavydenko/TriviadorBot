using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class Bot
	{
		private int _counter = 0; // Счетчик смены вопроса, каждые n раз - сохранение репозиториев		
		private readonly Process _process; // Процесс, за которым следим (Браузер, например)

		private readonly SyncMode mode; // Указывает на то, какой используется распознаватель (синхронный/асинхронный)
		private readonly IRecogniser _recogniser; // Синхронный распознаватель
		private readonly IRecogniserAsync _recogniserAsync; // Ассинхронный распознаватель

		private readonly IRepository<TestQuestion> _testRepository; // БД тестовых вопросов
		private readonly IRepository<NumericQuestion> _numericRepository; // БД циферных вопросов
		private readonly ICutter _cutter; // Разрезает Bitmap скриншота на фрагменты
		private readonly IClicker _clicker; // Кликает по правильному ответу и набирает число
		private readonly IReopener _reopener; // Пересоздает оконченную игру

		public Bitmap Screen { get; private set; } // Текущий скрин приложения
		public GameSituation Status { get; private set; } = GameSituation.Default; // Текущий игровой статус
		public bool IsGameStarted { get; private set; } = true; // Текущее состояние игры (запущена или нет)

		public int RightTestNumber { get; private set; }  // Текущий правильный в тестовом задании ответ (который подсветился)
		public string TestText { get; private set; } // Текст ТЕКУЩЕГО текстового вопроса
		public string Var1Text { get; private set; } // Текст ТЕКУЩЕГО текстового варианта 1
		public string Var2Text { get; private set; } // Текст ТЕКУЩЕГО текстового варианта 2
		public string Var3Text { get; private set; } // Текст ТЕКУЩЕГО текстового варианта 3
		public string Var4Text { get; private set; } // Текст ТЕКУЩЕГО текстового варианта 4

		public string NumericText { get; private set; } // Текст ТЕКУЩЕГО циферного вопроса	


		public Bot(int processId, IRecogniser recogniser, IRepository<TestQuestion> testRepository, IRepository<NumericQuestion> numberRepository, ICutter cutter, IClicker clicker, IReopener reopener)
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
			_clicker = clicker;
			_reopener = reopener;
		}

		public Bot(int processId, IRecogniserAsync recogniser, IRepository<TestQuestion> testRepository, IRepository<NumericQuestion> numberRepository, ICutter cutter, IClicker clicker, IReopener reopener)
		{
			_process = Process.GetProcesses().FirstOrDefault(p => p.Id == processId);
			if (_process == null)
			{
				Console.WriteLine("Процесс не найден");
				throw new Exception();
			}
			_recogniserAsync = recogniser;
			mode = SyncMode.Async; // Ассинхронный распознаватель

			_testRepository = testRepository;
			_numericRepository = numberRepository;
			_cutter = cutter;
			_clicker = clicker;
			_reopener = reopener;
		}

		private GameSituation Analyze(Bitmap src)
		{
			if (!IsGameStarted) // Если игра остановлена, то ничего не делаем
			{
				return GameSituation.Idle;
			}

			var thirteenthPixel = Screen.GetPixel(730, 795); // RGB от (23, 178, 23) до (99, 203, 99) на верхнем градиенте кнопки ОК после окончания игры
			var fourteenthPixel = Screen.GetPixel(730, 808); // RGB от (0, 153, 0) до (84, 186, 84) на нижнем градиенте кнопки ОК после окончания игры
			var fifteenthPixel = Screen.GetPixel(784, 671); // RGB (204, 204, 204) на щите первого места на пъедистале после окончания игры
			bool isGameFinished = ((thirteenthPixel.R == 23 && thirteenthPixel.G == 178 && thirteenthPixel.B == 23)
								|| (thirteenthPixel.R == 99 && thirteenthPixel.G == 203 && thirteenthPixel.B == 99))
								&& ((fourteenthPixel.R == 0 && fourteenthPixel.G == 153 && fourteenthPixel.B == 0)
								|| (fourteenthPixel.R == 84 && fourteenthPixel.G == 186 && fourteenthPixel.B == 84))
								&& fifteenthPixel.R == 204 && fifteenthPixel.G == 204 && fifteenthPixel.B == 204;
			if (isGameFinished) // Игра окончена
			{
				IsGameStarted = false;
				return GameSituation.Idle;
			}

			var firstPixel = src.GetPixel(1190, 468); // RGB (255, 255, 204) сверху справа
			var secondPixel = src.GetPixel(509, 517); // RGB (242, 233, 219) сверху слева
			var thirdPixel = src.GetPixel(1184, 816); // RGB (249, 245, 238) снизу справа
			var fourthPixel = src.GetPixel(998, 619); // RGB на тестовом вопросе от (255, 255, 255) до (250, 250, 250) - белый
			var fifthPixel = src.GetPixel(543, 643); // RGB (165, 88, 37) на краю дартца, если вопрос циферный

			bool isQuestion = firstPixel.R == 255 && firstPixel.G == 255 && firstPixel.B == 204
							&& secondPixel.R == 242 && secondPixel.G == 233 && secondPixel.B == 219
							&& thirdPixel.R == 249 && thirdPixel.G == 245 && thirdPixel.B == 238;

			if (isQuestion) // На экране вопрос (какой? еще неизвестно)
			{
				bool isTest = !(fifthPixel.R == 165 && fifthPixel.G == 88 && fifthPixel.B == 37); // Вопрос тестовый (не циферный)
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
						if (Status == GameSituation.AnswerTest) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Ответ
						{
							return GameSituation.AnswerTest;
						}
						return GameSituation.InactiveTest;
					}

					if (Status == GameSituation.AnswerTest) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Ответ
					{
						return GameSituation.AnswerTest;
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
						if (Status == GameSituation.AnswerNumeric) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Ответ
						{
							return GameSituation.AnswerNumeric;
						}
						return GameSituation.InactiveNumeric; // Неактивный циферный и без ответа
					}

					if (Status == GameSituation.AnswerNumeric) // Если прошлая ситуация - Ответ, то после нее не может быть (не)Активной, только Ответ
					{
						return GameSituation.AnswerNumeric;
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
			Console.Write("[Состояние]: ");
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
						Debug.WriteLine("Уже следим за этим вопросом -_-");
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

						// Лучше распознать варианты сразу, чтоб потом не было неточностей из-за того, что правильный вариант может подкраситься цветом
						if (mode == SyncMode.Sync)
							Var1Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 1));
						else if (mode == SyncMode.Async)
							Var1Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 1));

						if (mode == SyncMode.Sync)
							Var2Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 2));
						else if (mode == SyncMode.Async)
							Var2Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 2));

						if (mode == SyncMode.Sync)
							Var3Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 3));
						else if (mode == SyncMode.Async)
							Var3Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 3));

						if (mode == SyncMode.Sync)
							Var4Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 4));
						else if (mode == SyncMode.Async)
							Var4Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 4));


						if (status == GameSituation.ActiveTest) // Время отвечать 
						{
							Random rnd = new Random();
							_clicker.ClickTest(rnd.Next(1, 5)); // Рандомный клик							
						}
						return;
					}
					else // Вопрос уже есть в БД
					{
						// Лучше распознать варианты сразу, чтоб потом не было неточностей из-за того, что правильный вариант может подкраситься цветом
						if (mode == SyncMode.Sync)
							Var1Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 1));
						else if (mode == SyncMode.Async)
							Var1Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 1));

						if (mode == SyncMode.Sync)
							Var2Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 2));
						else if (mode == SyncMode.Async)
							Var2Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 2));

						if (mode == SyncMode.Sync)
							Var3Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 3));
						else if (mode == SyncMode.Async)
							Var3Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 3));

						if (mode == SyncMode.Sync)
							Var4Text = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, 4));
						else if (mode == SyncMode.Async)
							Var4Text = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, 4));

						if (status == GameSituation.ActiveTest) // Время отвечать
						{
							// Получаем правильный ответ из БД
							var rigthVar = question.GetRightOrDefault();
							if (rigthVar == null) // Такого не должно быть, но вдруг
							{
								Debug.WriteLine("Не смог найти ответ, однако в БД вопрос уже есть :(");
							}
							else // Ответ есть в БД
							{
								int rightNumber = 1;
								if (question.Answer == Var1Text)
									rightNumber = 1;
								else if (question.Answer == Var2Text)
									rightNumber = 2;
								else if (question.Answer == Var3Text)
									rightNumber = 3;
								else if (question.Answer == Var4Text)
									rightNumber = 4;

								Console.ForegroundColor = ConsoleColor.Green;
								Console.WriteLine("[Успех]: Помог ответ из БД");
								Console.ResetColor();
								_clicker.ClickTest(rightNumber);
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
					Debug.WriteLine("Не успели записать вопрос");
					return;
				}
				if (question.HasRight()) // Если уже есть ответ на этот вопрос
				{
					Debug.WriteLine("Ответ на этот вопрос уже есть в базе. Пропускаем");
					return;
				}

				string rightText = string.Empty; // Получаем текст правильного ответа
				if (mode == SyncMode.Sync)
					rightText = _recogniser.Recognise(_cutter.CutQuestionVariant(Screen, RightTestNumber));
				else if (mode == SyncMode.Async)
					rightText = await _recogniserAsync.Recognise(_cutter.CutQuestionVariant(Screen, RightTestNumber));

				if (string.IsNullOrEmpty(rightText) || string.IsNullOrWhiteSpace(rightText)) // Пустой ответ - ошибка распознавания
				{
					Debug.WriteLine("Не получилось распознать правильный вариант :(");
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
						Debug.WriteLine("Уже следим за этим вопросом -_-");
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
							if (numericText.ToLower().Contains("век") && !numericText.ToLower().Contains("человек"))
							{
								randomNumber = 18;
							}
							else if (numericText.ToLower().Contains("год"))
							{
								randomNumber = 1969;
							}
							_clicker.ClickNumeric(randomNumber);
						}
						return;
					}
					else // Вопрос уже есть в БД
					{
						if (status == GameSituation.ActiveNumeric) // Время отвечать
						{
							// Получаем правильный ответ из БД
							int? rightAnswer = question.GetRightOrDefault();
							if (rightAnswer == null) // Если цифра не распозналась в прошлый раз
							{
								Console.WriteLine("Не смог найти ответ, однако в БД вопрос уже есть :(");
								int randomNumber = 5; // Случайная цифра
													  // Псевдорандом по вхождению строк
								if (numericText.ToLower().Contains("век") && !numericText.ToLower().Contains("человек"))
								{
									randomNumber = 18;
								}
								else if (numericText.ToLower().Contains("год"))
								{
									randomNumber = 1969;
								}
								Console.ForegroundColor = ConsoleColor.Green;
								Console.WriteLine("[Успех]: Набрали ответ из БД");
								Console.ResetColor();
								_clicker.ClickNumeric(randomNumber);
							}
							else
							{
								_clicker.ClickNumeric(rightAnswer.Value);
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
					Debug.WriteLine("Не успели записали текст вопроса :(");
					return;
				}
				if (question.HasRight()) // Если уже есть ответ на этот вопрос
				{
					Debug.WriteLine("Ответ на этот вопрос уже есть в базе. Пропускаем");
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
					Debug.WriteLine("Не получилось распознать правильный вариант :(");
					return;
				}
				if (!isParsed)
				{
					Debug.WriteLine($"Не получилось получить целое число из строки {{{rightText}}}");
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
			int n = 2; // Каждые n добавленных вопросов происходит сохранение репозиториев. 2 - каждый раз
			if (_counter % n == 0 && _counter != 0)
			{
				_testRepository.Save();
				_numericRepository.Save();
				_counter++;
			}
		}

		private void ReopenGame()
		{
			_reopener.Reopen(); // Пересоздает игру
			IsGameStarted = true; // Игра запущена после перезапуска
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("[Система]: Игра начата!");
			Console.ResetColor();
		}

		public async Task StartAsync()
		{
			while (true)
			{
				try
				{
					Screen = _process.Screenshoot(); // Получение нового скрина приложения
					var status = Analyze(Screen); // Анализ нового скрина
					if (Status != status) // Если статус сменился
					{
						Say(status); // Описание статуса анализа 
					}
					Status = status;

					if (IsGameStarted) // Если игра начата
					{
						await Act(Screen, Status); // Действие на основе скрина и статуса
						SaveRepositories(); // Сохранение репозиториев
					}
					else // Нужно пересоздать игру (комнату)
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine("[Система]: Игра окончена!");
						Console.ResetColor();
						ReopenGame(); // Пересоздаем игру
					}

				}
				catch (ArgumentOutOfRangeException) // Если свернули приложение
				{
					Console.Write("\a");
				}
				await Task.Delay(300); // Задержка смены кадра
			}
		}

		private enum SyncMode
		{
			Sync,
			Async
		}
	}
}
