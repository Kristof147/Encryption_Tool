using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Security.Cryptography;
using Encryption_Tool.Service;

namespace Encryption_Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private Dictionary<string, string>? aesKeysDict;
		private readonly FileManager fm;
		string keyDirectoryPath = @"C:\test";
		string imageDirectoryPath = @"C:\test";
		string aESDirectoryPath = @"C:\test";
		public MainWindow()
		{
			InitializeComponent();
			InitializeKeys();
			fm = new FileManager();
		}

		private void btnKeyGeneration_Click(object sender, RoutedEventArgs e)
		{
			string publicKey;
			string privateKey;

			KeyHelper.SaveKeys(out publicKey, out privateKey);
			//txtPublicKey.Text = publicKey;
			//txtPrivateKey.Text = privateKey;
		}


		private void InitializeKeys()
		{
			aesKeysDict = new Dictionary<string, string>();


			if (Directory.Exists(keyDirectoryPath))
			{
				string[] files = Directory.GetFiles(keyDirectoryPath, "*.txt");

				foreach (string file in files)
				{
					string fileName = Path.GetFileName(file);
					string base64Key = File.ReadAllText(file).Trim();

					aesKeysDict.Add(fileName, base64Key);
					CmbAESKeys.Items.Add(fileName);
				}
			}

		}
		#region key generation tab
		private void BtnFolderKey_Click(object sender, RoutedEventArgs e)
		{
			string selectedDirectory = fm.SelectDirectory();
			if (!string.IsNullOrEmpty(selectedDirectory))
			{
				keyDirectoryPath = selectedDirectory;
				InitializeKeys();
			}
		}

		private void BtnAES_Click(object sender, RoutedEventArgs e)
		{

		}
		private void BtnRSAPair_Click(object sender, RoutedEventArgs e)
		{

		}



		#endregion
		#region AES tab
		private void BtnAESFolder_Click(object sender, RoutedEventArgs e)
		{
			string selectedDirectory = fm.SelectDirectory();
			if (!string.IsNullOrEmpty(selectedDirectory))
			{
				imageDirectoryPath = selectedDirectory;

			}
		}

		private void BtnAESEncrypt_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnAESDecrypt_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnAESImage_Click(object sender, RoutedEventArgs e)
		{

		}
		#endregion
		#region RSA tab
		private void BtnRSAEncrypt_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnRSADecrypt_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnRSAImage_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnRSAFolder_Click(object sender, RoutedEventArgs e)
		{
			string selectedDirectory = fm.SelectDirectory();
			if (!string.IsNullOrEmpty(selectedDirectory))
			{
				aESDirectoryPath = selectedDirectory;

			}
		}
		#endregion

		#region ImageToAndFromByteArray
		byte[] ImageToByteArray(string path)
		{
			return File.ReadAllBytes(path);
		}

		private BitmapImage ByteArrayToImage(byte[] data)
		{
			BitmapImage returnImage = new BitmapImage();
			Application.Current.Dispatcher.Invoke(() =>
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
		#endregion
	}
}