using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRecogniser.Models
{
	class Variant
	{
		public string Text { get; set; }		
		public bool IsRight { get; set; } = false;		

		public void SetRight() => IsRight = true;		
	}
}
