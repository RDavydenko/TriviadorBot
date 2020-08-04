namespace ConsoleRecogniser.Models
{
	public interface IClicker
	{
		void ClickTest(int answerNumber); // Кликнуть по правильному варианту ответа тестового вопроса по номеру answerNumber
		void ClickNumeric(int number); // Сделать несколько кликов, набирая правильное число number на активном циферном вопросе
	}
}
