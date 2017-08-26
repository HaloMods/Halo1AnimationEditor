using System;
using System.Collections;
using System.IO;

namespace HaloAnimationEditor
{
	/// <summary>
	/// Summary description for TagConverter.
	/// </summary>
	public class ValueSwap
	{
		public static string ToString(byte[] bytes)
		{
			string hexString = "";
			if ((object)bytes == null) return "";
			for (int i=0; i<bytes.Length; i++)
			{
				hexString += bytes[i].ToString("X2");
			}
			return hexString;
		}

		public static int BytesToInt(byte[] value)
		{
			string s = ValueSwap.ToString(value);
			return Convert.ToInt32(s, 16);
		}

		public static byte[] IntToBytes(int i)
		{
			string h = Convert.ToString(i, 16);
			int numOfZeroesNeeded = 8 - h.Length;
			for (int x=0; x<numOfZeroesNeeded; x++)
			{
				h = "0" + h;
			}
			int stuff;
			byte[] b = GetBytes(h, out stuff);
			return b;
		}

		/// <summary>
		/// Creates a byte array from the hexadecimal string. Each two characters are combined
		/// to create one byte. First two hexadecimal characters become first byte in returned array.
		/// Non-hexadecimal characters are ignored. 
		/// </summary>
		/// <param name="hexString">string to convert to byte array</param>
		/// <param name="discarded">number of characters in string ignored</param>
		/// <returns>byte array, in the same left-to-right order as the hexString</returns>
		public static byte[] GetBytes(string hexString, out int discarded)
		{
			discarded = 0;
			string newString = "";
			char c;
			// remove all none A-F, 0-9, characters
			for (int i=0; i<hexString.Length; i++)
			{
				c = hexString[i];
				if (IsHexDigit(c))
					newString += c;
				else
					discarded++;
			}
			// if odd number of characters, discard last character
			if (newString.Length % 2 != 0)
			{
				discarded++;
				newString = newString.Substring(0, newString.Length-1);
			}

			int byteLength = newString.Length / 2;
			byte[] bytes = new byte[byteLength];
			string hex;
			int j = 0;
			for (int i=0; i<bytes.Length; i++)
			{
				hex = new String(new Char[] {newString[j], newString[j+1]});
				bytes[i] = HexToByte(hex);
				j = j+2;
			}
			return bytes;
		}

		/// <summary>
		/// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
		/// </summary>
		/// <param name="c">Character to test</param>
		/// <returns>true if hex digit, false if not</returns>
		public static bool IsHexDigit(Char c)
		{
			int numChar;
			int numA = Convert.ToInt32('A');
			int num1 = Convert.ToInt32('0');
			c = Char.ToUpper(c);
			numChar = Convert.ToInt32(c);
			if (numChar >= numA && numChar < (numA + 6))
				return true;
			if (numChar >= num1 && numChar < (num1 + 10))
				return true;
			return false;
		}

		/// <summary>
		/// Converts 1 or 2 character string into equivalant byte value
		/// </summary>
		/// <param name="hex">1 or 2 character string</param>
		/// <returns>byte</returns>
		private static byte HexToByte(string hex)
		{
			if (hex.Length > 2 || hex.Length <= 0)
				throw new ArgumentException("hex must be 1 or 2 characters in length");
			byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return newByte;
		}

		
		#region Swapping
		public static short SwapShort(byte[] value)
		{
			byte[] temp = new byte[value.Length];
			temp[0] = value[1];
			temp[1] = value[0];
			return Convert.ToInt16(temp[0].ToString() + temp[1].ToString());
		}

		public static short ShortSwap( short s )
		{
			byte[] b = new byte[2];
  
			b[0] = Convert.ToByte(s & 255);
			b[1] = Convert.ToByte((s >> 8) & 255);

			return Convert.ToInt16((b[0] << 8) + b[1]);
		}

		public static int SwapLong(byte[] value)
		{
			byte[] temp = new byte[value.Length];
			temp[0] = value[3];
			temp[1] = value[2];
			temp[2] = value[1];
			temp[3] = value[0];
			return Convert.ToInt32(temp[0].ToString() + temp[1].ToString() + temp[2].ToString() + temp[3].ToString());
		}

		public static int SwapLong(int i)
		{
			byte[] b = new byte[4];

			b[0] = Convert.ToByte(i & 255);
			b[1] = Convert.ToByte((byte)( i >> 8 ) & 255);
			b[2] = Convert.ToByte((byte)( i>>16 ) & 255);
			b[3] = Convert.ToByte((byte)( i>>24 ) & 255);

			return ((int)b[0] << 24) + ((int)b[1] << 16) + ((int)b[2] << 8) + b[3];
		}
		#endregion

	}
}