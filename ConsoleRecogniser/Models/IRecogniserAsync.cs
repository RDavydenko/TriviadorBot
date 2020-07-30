using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	interface IRecogniserAsync
	{
		Task<string> Recognise(System.Drawing.Bitmap src);
		Task<string> RecogniseHard(System.Drawing.Bitmap src);
	}
}
