using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ConsoleRecogniser.Models
{
	class TestsRepository : IRepository<TestQuestion>
	{
		private List<TestQuestion> _db = new List<TestQuestion>();
		private string _path = "tests.json";

		public TestsRepository()
		{
			Initialize();
			if (_db == null)
			{
				_db = new List<TestQuestion>();
			}
		}

		private void Initialize()
		{
			if (File.Exists(_path))
			{
				string serialized = File.ReadAllText(_path);
				_db = JsonConvert.DeserializeObject<List<TestQuestion>>(serialized);
				Console.WriteLine($"Загружены тестовые вопросы: {(_db != null ? _db.Count : 0)} шт.");
			}
			else
			{
				File.Create(_path);
			}
		}

		public void Add(TestQuestion item)
		{
			_db.Add(item);
			Console.WriteLine($"Добавлена новая запись: {item.Text.Trim('\r', '\n', '\t', ' ', ' ')}");
		}

		public TestQuestion FirstOrDefault(string text)
		{
			return _db.FirstOrDefault(t => t.Text == text && !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text));
		}

		public void Save()
		{
			string serialized = JsonConvert.SerializeObject(_db.Where(q => q.Answer != string.Empty)); // Сохраняет только не с пустым ответом
			var streamWriter = new StreamWriter(new FileStream(_path, FileMode.Create, FileAccess.Write));
			streamWriter.Write(serialized);
			streamWriter.Close();
		}
	}
}
