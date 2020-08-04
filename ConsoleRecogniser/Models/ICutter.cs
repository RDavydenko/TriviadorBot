namespace ConsoleRecogniser.Models
{
	public interface ICutter
	{
		System.Drawing.Bitmap CutQuestionText(System.Drawing.Bitmap src);
		System.Drawing.Bitmap CutQuestionVariant(System.Drawing.Bitmap src, int number);
		System.Drawing.Bitmap CutNumericText(System.Drawing.Bitmap src);
		System.Drawing.Bitmap CutNumericAnswer(System.Drawing.Bitmap src);

	}
}
