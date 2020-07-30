using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	public enum GameSituation
	{
		Default, // Без состояния
		Idle, // Простой
		ActiveTest, // Активный тестовый вопрос
		InactiveTest, // Неактивный тестовый вопрос
		AnswerTest, // Ответ тестового вопроса
		ActiveNumeric, // Активный циферный вопрос
		InactiveNumeric, // Неактивный циферный вопрос
		AnswerNumeric // Ответ циферного вопроса
	}
}
