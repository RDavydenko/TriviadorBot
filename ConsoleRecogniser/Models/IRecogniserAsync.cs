using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	interface IRecogniserAsync
	{
		Task<string> Recognise(System.Drawing.Bitmap src);
		Task<string> RecogniseHard(System.Drawing.Bitmap src);
	}
}
