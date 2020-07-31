using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class ReopenerWithBots : IReopener
	{
		private GameTheme _gameTheme;  // Тема игры
		private ClickerFHD _clicker;  // Кликер

		public ReopenerWithBots(int processId, GameTheme gameTheme = GameTheme.All)
		{
			_gameTheme = gameTheme;
			_clicker = new ClickerFHD(processId);
		}

		// Пересоздает комнату
		public void Reopen()
		{
			_clicker.Click(795, 800); // Клик по кнопке ОК
			Task.Delay(10000).Wait(); // Ждем немного

			_clicker.Click(870, 600); // Клик по кнопке Дружеская игра
			Task.Delay(10000).Wait(); // Ждем немного

			_clicker.Click(590, 555); // Клик по кнопке Добавить бота 1
			Task.Delay(2000).Wait(); // Ждем немного

			_clicker.Click(590, 555); // Клик по кнопке Добавить бота 2
			Task.Delay(2000).Wait(); // Ждем немного

			ChooseThemes(_gameTheme); // Выбираем тему

			_clicker.Click(810, 800); // Клик по кнопке Создать игру
			Task.Delay(10000).Wait(); // Ждем немного

			_clicker.Click(1024, 800); // Клик по кнопке Начать игру
		}

		// Клик в зависимости от выбора темы
		private void ChooseThemes(GameTheme theme)
		{
			if (theme == GameTheme.All) // Не делаем ничего, потому что они уже все выбраны изначально
			{
				return;
			}
			else
			{
				_clicker.Click(780, 690); // Клик по кнопке Убрать все темы
				switch (theme)
				{					
					case GameTheme.Art:
						_clicker.Click(463, 670); // Клик по кнопке Искусство
						break;
					case GameTheme.Media:
						_clicker.Click(522, 670); // Клик по кнопке СМИ
						break;
					case GameTheme.Geography:
						_clicker.Click(590, 670); // Клик по кнопке География
						break;
					case GameTheme.Wars:
						_clicker.Click(654, 670); // Клик по кнопке Войны
						break;
					case GameTheme.Books:
						_clicker.Click(715, 670); // Клик по кнопке Книги
						break;

					case GameTheme.Mathematics:
						_clicker.Click(463, 710); // Клик по кнопке Математика
						break;
					case GameTheme.Biology:
						_clicker.Click(522, 710); // Клик по кнопке Биология
						break;
					case GameTheme.Sport:
						_clicker.Click(590, 710); // Клик по кнопке Спорт
						break;
					case GameTheme.Theatre:
						_clicker.Click(654, 710); // Клик по кнопке Театр
						break;
					case GameTheme.Beauty:
						_clicker.Click(715, 710); // Клик по кнопке Красота
						break;
					default:
						break;
				}
				return;
			}
		}

		public enum GameTheme
		{
			All, // Все
			Art, // Искусство
			Media, // СМИ
			Geography, // География
			Wars, // Конфликты
			Books, // Книги
			Mathematics, // Математика
			Biology, // Биология
			Sport, // Спорт
			Theatre, // Театр
			Beauty // Красота
		}
	}
}
