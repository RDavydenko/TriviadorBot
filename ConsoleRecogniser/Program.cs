using System;
using ConsoleRecogniser.Models;

namespace ConsoleRecogniser
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Введите ID процесса браузера Chrome: ");
			string processId = Console.ReadLine();

			if (!int.TryParse(processId, out int id))
			{
				Console.WriteLine("Некорректный ID процесса");
				return;
			}

			Bot bot = new Bot(
				processId: id,
				recogniser: new RecogniserAsync("rusf"),
				testRepository: new TestsRepository(),
				numberRepository: new NumericsRepository(),
				cutter: new CutterFHD(),
				clicker: new ClickerFHD(id),
				reopener: new ReopenerWithBots(id, ReopenerWithBots.GameTheme.Art)
			);

			bot.StartAsync().Wait();
		}
	}
}
