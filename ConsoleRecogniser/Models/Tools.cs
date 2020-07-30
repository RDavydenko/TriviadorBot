using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ConsoleRecogniser.Models
{
	public static class Tools
	{
		public static Bitmap Screenshoot(this Process process)
		{
			var hwnd = process.MainWindowHandle;
			GetWindowRect(hwnd, out RECT rect);
			Bitmap image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
			using (var g = Graphics.FromImage(image))
			{
				var hdcBitmap = g.GetHdc();
				PrintWindow(hwnd, hdcBitmap, 0);
				g.ReleaseHdc(hdcBitmap);
			}
			GC.Collect();
			return image;
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll")]
		private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left, Top, Right, Bottom;
		}
	}
}
