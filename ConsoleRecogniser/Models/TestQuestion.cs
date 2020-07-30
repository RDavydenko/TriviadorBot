using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class TestQuestion
	{
		public string Text { get; set; }

		public string Answer { get; set; } = string.Empty;

		public bool HasRight()  // Имеется ли правильный вариант?
		{
			return Answer != string.Empty;
		}

		public void SetRight(string text) // Устанавливает правильный вариант
		{
			if (HasRight())
			{
				throw new Exception("Уже имеем правильный вариант! Нельзя установить несколько!");
			}
			Answer = text;
		}

		public string GetRightOrDefault() // Возвращает правильный ответ или null
		{
			if (Answer != null)
			{
				return Answer;
			}
			return null;

		}

	}
}
