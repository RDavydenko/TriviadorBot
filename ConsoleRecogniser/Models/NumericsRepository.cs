using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleRecogniser.Models
{
	class NumericsRepository : IRepository<NumericQuestion>
	{
		private List<NumericQuestion> _db = new List<NumericQuestion>();
		private string _path = "numerics.json";

		public NumericsRepository()
		{
			Initialize();
			if (_db == null)
			{
				_db = new List<NumericQuestion>();
			}
		}

		private void Initialize()
		{
			if (File.Exists(_path))
			{
				string serialized = File.ReadAllText(_path);
				_db = JsonConvert.DeserializeObject<List<NumericQuestion>>(serialized);
				Console.WriteLine($"Загружены циферные вопросы: {(_db != null ? _db.Count : 0)} шт.");
			}
			else
			{
				File.Create(_path);
			}
		}


		public void Add(NumericQuestion item)
		{
			_db.Add(item);
			Console.WriteLine($"Добавлена новая запись: {item.Text.Trim('\r', '\n', '\t', ' ', ' ')}");
		}

		public NumericQuestion FirstOrDefault(string text)
		{
			return _db.FirstOrDefault(q => q.Text == text);
		}

		public void Save()
		{
			string serialized = JsonConvert.SerializeObject(_db); // Сохраняет ВСЕ, даже пустые, чтоб потом можно было подправить вручную (дописать числа int)
			var streamWriter = new StreamWriter(new FileStream(_path, FileMode.Create, FileAccess.Write));
			streamWriter.Write(serialized);
			streamWriter.Close();
			Console.WriteLine("Прошло сохранение циферных вопросов...");
		}
	}
}
