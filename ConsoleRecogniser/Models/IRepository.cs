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
