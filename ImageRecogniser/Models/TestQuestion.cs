using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRecogniser.Models
{
	class TestQuestion
	{
		public string Text { get; set; }

		public Variant Variant1 { get; set; }
		public Variant Variant2 { get; set; }
		public Variant Variant3 { get; set; }
		public Variant Variant4 { get; set; }

		public bool HasRight() // Имеется ли среди вариантов правильный?
		{
			return (Variant1.IsRight || Variant2.IsRight || Variant3.IsRight ||Variant4.IsRight);
		}

		public Variant GetRightOrDefault() // Возвращает правильный ответ или null
		{
			if (Variant1.IsRight)
			{
				return Variant1;
			}
			else if (Variant2.IsRight)
			{
				return Variant2;
			}
			else if (Variant3.IsRight)
			{
				return Variant3;
			}
			else if (Variant4.IsRight)
			{
				return Variant4;
			}
			else
			{
				return null;
			}
		}

	}
}
