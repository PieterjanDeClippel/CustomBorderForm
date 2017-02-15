using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bordertest
{
	public partial class CustomForm : Form
	{
		public CustomForm()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}
		
		bool active = false;
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case Win32.WM_NCACTIVATE:
					break;
				case Win32.WM_ACTIVATE:
					active = m.WParam != Win32.WA_INACTIVE;
					break;
				case Win32.WM_NCLBUTTONDOWN:
					if (m.LParam != IntPtr.Zero)
					{
						Point co = new Point((int)m.LParam);
						co.Offset(-Location.X, -Location.Y);
						if ((co.X > Width - 50 - 0 * 42) & (co.Y <= 20))
						{
							Close();
						}
						else if ((co.X > Width - 50 - 1 * 42) & (co.Y <= 20))
						{
							WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
						}
						else if ((co.X > Width - 50 - 2 * 42) & (co.Y <= 20))
						{
							WindowState = FormWindowState.Minimized;
						}
						else
							base.WndProc(ref m);
					}
					break;
				case 0xae:
					break;
				case Win32.WM_NCPAINT:
					try
					{
						IntPtr hdc = Win32.GetWindowDC(m.HWnd);
						if (hdc != IntPtr.Zero)
						{
							#region Create Graphics
							Graphics gr = Graphics.FromHdc(hdc);
							//BufferedGraphics bgr = BufferedGraphicsManager.Current.Allocate(gr2, new Rectangle(0, 0, Width, Height));
							//Graphics gr = bgr.Graphics;
							#endregion
							#region Draw Frame
							ColorConverter cc = new ColorConverter();
							Color c1 = (Color)cc.ConvertFromString("#FF588391");
							Color c2 = (Color)cc.ConvertFromString("#FF7AA5B3");
							Color c3 = (Color)cc.ConvertFromString("#FF9CC7D5");
							LinearGradientBrush br_title = new LinearGradientBrush(new Point(), new Point(0, 40), c1, c2);
							LinearGradientBrush br_frame = new LinearGradientBrush(new Point(0, 40), new Point(0, Height), c2, c3);

							gr.FillRectangle(br_title, new Rectangle(0, 0, Width, 40));

							Rectangle rct = new Rectangle(10, 39, Width - 20, Height - 49);
							Region reg = new Region(new Rectangle(0, 40, Width, Height - 40));
							reg.Exclude(rct);
							gr.FillRegion(br_frame, reg);
							#endregion
							#region Draw Icon
							if (ShowIcon)
								gr.DrawIcon(Icon, new Rectangle(8, 8, 16, 16));
							#endregion
							#region Draw ControlBox
							for(int i = 0; i<3;i++)
							{
								PathGradientBrush br_buttons = new PathGradientBrush(new Point[] { new Point(Width - 50 - i * 42, 0), new Point(Width - 10 - i * 42, 0), new Point(Width - 10 - i * 42, 20), new Point(Width - 50 - i * 42, 20) });
								br_buttons.CenterColor = i == 0 ? Color.Orange : Color.Blue;
								br_buttons.SurroundColors = new Color[] { i == 0 ? Color.Red : Color.DarkBlue };
								GraphicsPath p = GetButtonPath(Width - 50 - i * 42, 0, 40, 20, 8);
								gr.FillPath(br_buttons, p);
								gr.DrawPath(Pens.Black, p);
							}
							Pen pen = new Pen(Color.White, 3) { StartCap = LineCap.Round, EndCap = LineCap.Round };
							gr.DrawLine(pen, Width - 50 + 20 - 5, 10 - 5, Width - 50 + 20 + 5, 10 + 5);
							gr.DrawLine(pen, Width - 50 + 20 - 5, 10 + 5, Width - 50 + 20 + 5, 10 - 5);
							gr.DrawLine(pen, Width - 50 - 2 * 42 + 10, 15, Width - 50 - 2 * 42 + 20, 15);
							pen.Width = 2;
							gr.DrawRectangle(pen, Width - 50 - 1 * 42 + 12, 5, 16, 10);

							#endregion
							#region Clean up
							//bgr.Render();
							
							Win32.ReleaseDC(m.HWnd, hdc);
							#endregion
						}
						m.Result = (IntPtr)1;
					}
					catch (Exception)
					{ }
					break;
				case Win32.WM_NCCALCSIZE:
					if(m.WParam != IntPtr.Zero)
					{
						Win32.NCCALCSIZE_PARAMS rcsize = (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));
						AdjustClientRect(ref rcsize.rcNewWindow);
						Marshal.StructureToPtr(rcsize, m.LParam, false);
					}
					else
					{
						Win32.RECT rcsize = Marshal.PtrToStructure<Win32.RECT>(m.LParam);
						AdjustClientRect(ref rcsize);
						Marshal.StructureToPtr(rcsize, m.LParam, false);
					}
					m.Result = (IntPtr)1;
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}

		private GraphicsPath GetButtonPath(int x,int y, int width, int height, int r)
		{
			int b = 2 * r;
			GraphicsPath p = new GraphicsPath();
			p.AddLine(x, y, x, y + height - r);
			p.AddArc(x, y + height - b, b, b, 180, -90);
			p.AddLine(x + r, y + height, x + width - r, y + height);
			p.AddArc(x + width - b, y + height - b, b, b, 90, -90);
			p.AddLine(x + width, y + height - r, x + width, y);
			p.CloseFigure();
			return p;
		}

		private void AdjustClientRect(ref Win32.RECT rcClient)
		{
			rcClient.Left += 10;
			rcClient.Top += 40;
			rcClient.Right -= 10;
			rcClient.Bottom -= 10;
		}

#if DropShadow
		public static bool DropShadowSupported
		{
			get
			{
				bool runningNT = Environment.OSVersion.Platform == PlatformID.Win32NT;
				return runningNT && Environment.OSVersion.Version.CompareTo(new Version(5, 1, 0, 0)) >= 0;
			}
		}

		private const int CS_DROPSHADOW = 0x00020000;
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;

				if (DropShadowSupported)
					p.ClassStyle = (p.ClassStyle | CS_DROPSHADOW);

				return p;
			}
		}
#endif
	}
}
