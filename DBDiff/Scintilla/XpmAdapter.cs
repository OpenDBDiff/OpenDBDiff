using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Converts Bitmap images to XPM data for use with ScintillaNet.
/// Warning: images with more than (around) 50 colors will generate incorrect XPM
/// The XpmConverter class was based on code from flashdevelop. 
/// </summary>
public static class XpmConverter
{
	/// <summary>
	/// The default transparent Color
	/// </summary>
	public static string DefaultTransparentColor = "#FF00FF";

	/// <summary>
	/// cicles an image list object to convert contained images into xpm
	/// at the same time we add converted images into an arraylist that lets us to retrieve images later.
	/// Uses the DefaultTransparentColor.
	/// </summary>
	/// <param name="imageList">The image list to transform.</param>
	/// <returns></returns>
	static public List<string> ConvertToXPM(ImageList ImageList)
	{
		return ConvertToXPM(ImageList, DefaultTransparentColor);
	}

	/// <summary>
	/// cicles an image list object to convert contained images into xpm
	/// at the same time we add converted images into an arraylist that lets us to retrieve images later	
	/// </summary>
	/// <param name="imageList">The image list to transform.</param>
	/// <param name="transparentColor">The overriding transparent Color</param>
	/// <returns></returns>
	static public List<string> ConvertToXPM(ImageList imageList, string transparentColor)
	{
		List<string> xpmImages = new List<string>();
		foreach (Image image in imageList.Images)
		{
			if (image is Bitmap)
			{
				xpmImages.Add(ConvertToXPM(image as Bitmap, transparentColor));
			}
		}
		return xpmImages;
	}

	/// <summary>
	/// Converts Bitmap images to XPM data for use with ScintillaNet.
	/// Warning: images with more than (around) 50 colors will generate incorrect XPM.
	/// Uses the DefaultTransparentColor.
	/// </summary>
	/// <param name="bmp">The image to transform.</param>
	/// <returns></returns>
	static public string ConvertToXPM(Bitmap bmp)
	{
		return ConvertToXPM(bmp, DefaultTransparentColor);
	}

	/// <summary>
	/// Converts Bitmap images to XPM data for use with ScintillaNet.
	/// Warning: images with more than (around) 50 colors will generate incorrect XPM
	/// tColor: specified transparent color in format: "#00FF00".
	/// </summary>
	/// <param name="bmp">The image to transform.</param>
	/// <param name="transparentColor">The overriding transparent Color</param>
	/// <returns></returns>
	static public string ConvertToXPM(Bitmap bmp, string transparentColor)
	{
		StringBuilder sb = new StringBuilder();
		List<string> colors = new List<string>();
		List<char> chars = new List<char>();
		int width = bmp.Width;
		int height = bmp.Height;
		int index;
		sb.Append("/* XPM */static char * xmp_data[] = {\"").Append(width).Append(" ").Append(height).Append(" ? 1\"");
		int colorsIndex = sb.Length;
		string col;
		char c;
		for (int y = 0; y < height; y++)
		{
			sb.Append(",\"");
			for (int x = 0; x < width; x++)
			{
				col = ColorTranslator.ToHtml(bmp.GetPixel(x, y));
				index = colors.IndexOf(col);
				if (index < 0)
				{
					index = colors.Count + 65;
					colors.Add(col);
					if (index > 90) index += 6;
					c = Encoding.ASCII.GetChars(new byte[] { (byte)(index & 0xff) })[0];
					chars.Add(c);
					sb.Insert(colorsIndex, ",\"" + c + " c " + col + "\"");
					colorsIndex += 14;
				}
				else c = (char)chars[index];
				sb.Append(c);
			}
			sb.Append("\"");
		}
		sb.Append("};");
		string result = sb.ToString();
		int p = result.IndexOf("?");
		string finalColor = result.Substring(0, p) + colors.Count + result.Substring(p + 1).Replace(transparentColor.ToUpper(), "None");

		return finalColor;
	}
}