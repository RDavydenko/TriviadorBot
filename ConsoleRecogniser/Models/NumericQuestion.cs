using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class NumericQuestion
	{
		public string Text { get; set; }
		public int? Number { get; private set; } = null;

		public void SetRight(int answerNumber)
		{
			Number = answerNumber;
		}

		public bool HasRight()
		{
			return Number.HasValue;
		}

		public int? GetRightOrDefault()
		{
			return Number;
		}
	}
}
