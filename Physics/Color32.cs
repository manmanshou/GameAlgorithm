using System.Runtime.InteropServices;
using System.Text;

namespace Y7Engine
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Color32
	{
		public static readonly Color32 Black = new Color32();
		public static readonly Color32 DarkBlue = new Color32(0xFF2484BC);
		public static readonly Color32 DarkGreen = new Color32(0xFF00A800);
		public static readonly Color32 DarkCyan = new Color32(0xFF009696);
		public static readonly Color32 DarkRed = new Color32(0xFFD82424);
		public static readonly Color32 DarkMagenta = new Color32(0xFFA8366C);
		public static readonly Color32 DarkYellow = new Color32(0xFFB68A00);
		public static readonly Color32 Gray = new Color32(0xFFB0B0B0);
		public static readonly Color32 DarkGray = new Color32(0xFF666666);
		public static readonly Color32 Blue = new Color32(0xFF209AFF);
		public static readonly Color32 Green = new Color32(0xFF4CD44C);
		public static readonly Color32 Cyan = new Color32(0xFF12B6CA);
		public static readonly Color32 Red = new Color32(0xFFFF5644);
		public static readonly Color32 Magenta = new Color32(0xFFF24CF8);
		public static readonly Color32 Yellow = new Color32(0xFFFFB624);
		public static readonly Color32 White = new Color32(0xFFD8D8D8);
		public static readonly Color32 Purple = new Color32(0xFFF066F0);
		public static readonly Color32 DarkPurple = new Color32(0xFFB624B6);
		public static readonly Color32 Orange = new Color32(0xFFF0A050);
		public static readonly Color32 DarkOrange = new Color32(0xFFF07024);

		public byte B;
		public byte G;
		public byte R;
		public byte A;
		string codeStr;

		public Color32(byte a, byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
			this.codeStr = null;
		}
		public Color32(byte r, byte g, byte b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 255;
			this.codeStr = null;
		}
		public unsafe Color32(uint color)
		{
			this.R = 0;
			this.G = 0;
			this.B = 0;
			this.A = 0;
			this.codeStr = null;
			fixed (byte* ptr = &this.B)
			{
				*(uint*)ptr = color;
			}
		}

		public override string ToString()
		{
			if (codeStr == null)
			{
				StringBuilder sb = new StringBuilder(9);
				sb.Append('#');
				sb.Append(A.ToString("X2"));
				sb.Append(R.ToString("X2"));
				sb.Append(G.ToString("X2"));
				sb.Append(B.ToString("X2"));
				codeStr = sb.ToString();
			}
			return codeStr;
		}

		//public string toString()
		//{
		//	StringBuilder sb = new StringBuilder(7);
		//	sb.Append('#');
		//	sb.Append(R.ToString("X2"));
		//	sb.Append(G.ToString("X2"));
		//	sb.Append(B.ToString("X2"));
		//	codeStr = sb.ToString();
		//	return codeStr;
		//}
	}
}
