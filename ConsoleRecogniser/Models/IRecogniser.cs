namespace ConsoleRecogniser.Models
{
	public interface IRecogniser
	{
		string Recognise(System.Drawing.Bitmap src);

		string RecogniseHard(System.Drawing.Bitmap src);
	}
}
