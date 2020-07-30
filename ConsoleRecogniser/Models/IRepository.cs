using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	public interface IRepository<T> 
		where T : class
	{
		void Save();
		void Add(T item);
		T FirstOrDefault(string text);
	}
}
