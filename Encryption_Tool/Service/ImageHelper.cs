using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Encryption_Tool.Service
{
	public static class ImageHelper
	{
		public static string LoadImage()
		{
			OpenFileDialog ofd = new();
			ofd.Filter = "PNG Files|*.png";
			if (ofd.ShowDialog() == DialogResult.OK)
				return ofd.FileName;
			
			return string.Empty;
		}

		public static byte[] ImageToByteArray(string path)
		{
			return File.ReadAllBytes(path);
		}

		public static BitmapImage ByteArrayToImage(byte[] data)
		{
			BitmapImage returnImage = new BitmapImage();
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				MemoryStream stream = new MemoryStream();
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				returnImage.BeginInit();
				returnImage.CacheOption = BitmapCacheOption.OnLoad;
				returnImage.StreamSource = stream;
				returnImage.EndInit();
				returnImage.Freeze(); // Zorgt ervoor dat de afbeelding niet verandert nadat het is geladen
			});
			return returnImage;
		}
	}
}
