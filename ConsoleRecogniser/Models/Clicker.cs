using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class Clicker : IClicker
	{
		public void ClickTest(int answerNumber)
		{
			Console.WriteLine("Здесь должен быть клик по номеру " + answerNumber);
		}

		public void ClickNumeric(int number)
		{
			Console.WriteLine("Здесь нужно набрать число " + number);
		}
	}
}
