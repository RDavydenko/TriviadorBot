using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	public interface IRecogniser
	{
		string Recognise(System.Drawing.Bitmap src);

		string RecogniseHard(System.Drawing.Bitmap src);
	}
}
