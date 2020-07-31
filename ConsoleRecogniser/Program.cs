using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

			Bot bot = new Bot(id, new RecogniserAsync("rusf"), new TestsRepository(), new NumericsRepository(), new CutterFHD(), new Clicker(id));
			bot.StartAsync().Wait();
		}
	}
}
