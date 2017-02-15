using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace bordertest
{
	public static class Win32
	{
		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("User32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

		}
		
		public struct NCCALCSIZE_PARAMS
		{
			public RECT rcNewWindow;
			public RECT rcOldWindow;
			public RECT rcClient;
			IntPtr lppos;
		}


		public const int WM_PAINT = 0x000F;
		public const int WM_NCPAINT = 0x0085;
		public const int WM_NCACTIVATE = 0x0086;
		public const int WM_ERASEBKGND = 0x0014;
		public const int WM_ACTIVATE = 0x0006;
		public const int WM_NCCALCSIZE = 0x0083;
		public const int WM_NCLBUTTONDOWN = 0x00A1;
		public const int WM_COMMAND = 0x0111;
		public const int WM_NCHITTEST = 0x0084;
		public static IntPtr WA_INACTIVE = IntPtr.Zero;
	}
}
