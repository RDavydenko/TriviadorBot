namespace ImageRecogniser.Models
{
	class Variant
	{
		public string Text { get; set; }
		public bool IsRight { get; set; } = false;

		public void SetRight() => IsRight = true;
	}
}
