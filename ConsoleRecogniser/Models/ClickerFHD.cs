using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRecogniser.Models
{
	class ClickerFHD : IClicker
	{
        private Process _process;

        private const int
            WM_LBUTTONDOWN = 513,
            WM_LBUTTONUP = 514,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_KEYDOWN = 256,
            WM_CHAR = 258,
            WM_KEYUP = 257,
            WM_SETFOCUS = 7,
            WM_SYSCOMMAND = 274,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_CLEAR = 0x303,
            WM_PAINT = 15,
            WM_SETCURSOR = 32,
            WM_KILLFOCUS = 8,
            WM_NCHITTEST = 132,
            WM_USER = 1024,
            WM_MOUSEACTIVATE = 33,
            WM_MOUSEMOVE = 512,
            WM_LBUTTONDBLCLK = 515,
            WM_COMMAND = 273,
            VK_DOWN = 0x28,
            VK_RETURN = 0x0D,
            BM_SETSTATE = 243,
            BM_CLICK = 0x00F5,
            SW_HIDE = 0,
            SW_MAXIMIZE = 3,
            SW_MINIMIZE = 6,
            SW_RESTORE = 9,
            SW_SHOW = 5,
            SW_SHOWDEFAULT = 10,
            SW_SHOWMAXIMIZED = 3,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOWNORMAL = 1,
            SC_MINIMIZE = 32,
            EM_SETSEL = 0x00B1,
            CAPACITY = 256,
            CB_SETCURSEL = 0x014E;

		public ClickerFHD(int processId)
		{
            _process = Process.GetProcesses().FirstOrDefault(p => p.Id == processId);
            if (_process == null)
			{
				Console.WriteLine("[Clicker]: Неверный ID процесса");
                throw new Exception();
			}            
		}

        /* клик левой клавишей мыши по координатам */
        private void ClickMouseLeft(IntPtr hwnd, int x, int y)
        {
            SendMessage(hwnd, WM_LBUTTONDOWN, IntPtr.Zero, new IntPtr((y * 0x10000 + x)));
            SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, new IntPtr(y * 0x10000 + x));
        }

      
        // Клик по клавишам на клавиатуре циферного вопроса
        private void ClickOnNumber(int num)
        {
			switch (num)
			{
                
                case 0:
                    Click(880, 592);
                    break;

                case 1:
                    Click(603, 765);
                    break;
                case 2:
                    Click(690, 765);
                    break;
                case 3:
                    Click(783, 765);
                    break;

                case 4:
                    Click(603, 685);
                    break;
                case 5:
                    Click(690, 685);
                    break;
                case 6:
                    Click(783, 685);
                    break;

                case 7:
                    Click(603, 592);
                    break;
                case 8:
                    Click(690, 592);
                    break;
                case 9:
                    Click(783, 592);
                    break;
                
                default:
					break;
			}
		}

        // Клик по клавише отправки циферного вопроса
        private void ClickOnSendButton()
        {
            Click(920,730);
        }

        // Разбить число на разряды, с учетом нуля
        private int[] SplitIntoDigits(int number)
        {
            List<int> result = new List<int>();
            for (int i = 0; number > 0; i++)
            {
                int digit = number % 10;
                number /= 10;
                result.Insert(0, digit);
            }
            if (result.Count == 0)
			{
                result.Add(0);
			}
            return result.ToArray();
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


        // Клик по координатам
        public void Click(int x, int y)
        {
            IntPtr hwnd = _process.MainWindowHandle;
            ClickMouseLeft(hwnd, x, y);
        }

        // Клик по ответу тестового вопроса с номером ответа answerNumber
        public void ClickTest(int answerNumber)
		{
            switch (answerNumber)
            {
                case 1:
                    Click(800, 543);
                    break;
                case 2:
                    Click(800, 621);
                    break;
                case 3:
                    Click(800, 705);
                    break;
                case 4:
                    Click(800, 788);
                    break;
                default:
                    break;
            }
            Console.WriteLine("Здесь должен быть клик по номеру " + answerNumber);
		}

        // Набрать на циферной клавиатуре число number для циферного вопроса
		public void ClickNumeric(int number)
		{
            int[] nums = SplitIntoDigits(number); // Разбиваем на разряды число, которое нужно набрать
            foreach (int num in nums)
			{
                ClickOnNumber(num);
                Task.Delay(50).Wait();
			}
            ClickOnSendButton();
			Console.WriteLine("Здесь нужно набрать число " + number);
		}
	}
}
