using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	interface IAsyncRecogniser
	{
		Task<string> Recognise(System.Drawing.Bitmap src);
		Task<string> RecogniseHard(System.Drawing.Bitmap src);
	}
}
