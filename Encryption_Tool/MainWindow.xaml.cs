using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
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
using System.Windows.Shapes;

using System.Security.Cryptography;
using Encryption_Tool.Service;
using System.Drawing;
using System.IO;
using Image = System.Drawing.Image;

namespace Encryption_Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			EncryptionEngine.CryptoEngine engine = new();
			RSAParameters encryptionParameters = new();
			RSAParameters decryptionParameters = new();
			RSAEncryptionPadding padding = RSAEncryptionPadding.CreateOaep(HashAlgorithmName.SHA256);
			using (RSA Rsa = RSA.Create())
			{
				encryptionParameters = Rsa.ExportParameters(false);
				decryptionParameters = Rsa.ExportParameters(true);
			}
			UnicodeEncoding byteConverter = new();
			byte[] dataToEncrypt = byteConverter.GetBytes("This is a test");
			CryptoParameters parameters = new()
			{
				RSAParameters = encryptionParameters,
				RSAEncryptionPadding = padding,
			};
			EncryptionRequest request = new()
			{
				DataToEncrypt = dataToEncrypt,
				EncryptionType = EncryptionType.RSA,
				Parameters = parameters
			};
			var response = engine.Encrypt(request);
			parameters.RSAParameters = decryptionParameters;
			DecryptionRequest decryptRequest = new()
			{
				DataToDecrypt = response.Data,
				EncryptionType = EncryptionType.RSA,
				Parameters = parameters
			};
			var decryptResponse = engine.Decryption(decryptRequest);
			string decryptedData = byteConverter.GetString(decryptResponse.Data);
			_ = 0;

			Aes aes = Aes.Create();
			aes.GenerateKey();
			aes.GenerateIV();
			// ToBase64
			string keyAesBase64 = Convert.ToBase64String(aes.Key);
			string ivAesBase64 = Convert.ToBase64String(aes.IV);
			// FromBase64
			var key = Convert.FromBase64String(keyAesBase64);
			var iv = Convert.FromBase64String(ivAesBase64);

			//if (aes.Key.SequenceEqual(key) && aes.IV.SequenceEqual(iv))
			//	MessageBox.Show("ok");
			//else
			//	MessageBox.Show("nok");

			CryptoParameters cryptoParameters = new()
			{
				Aes = aes
			};
			EncryptionRequest requestAes = new()
			{
				DataToEncrypt = dataToEncrypt,
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			response = engine.Encrypt(requestAes);
			DecryptionRequest decryptionRequestAes = new()
			{
				DataToDecrypt = response.Data,
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			var decryptResponse2 = engine.Decryption(decryptionRequestAes);
			var decryptedData2 = byteConverter.GetString(decryptResponse2.Data);
			_ = 0;

			string imgPath = @"C:\Users\12201505\OneDrive - PXL\Afbeeldingen\Portfolio-ideas\cv idea 1.png";
			EncryptionRequest imgRequest = new()
			{
				DataToEncrypt = ImageToByteArray(imgPath),
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			response = engine.Encrypt(imgRequest);
			DecryptionRequest imgDecryptionRequest = new()
			{
				DataToDecrypt = response.Data,
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			var imgDecryptResponse = engine.Decryption(imgDecryptionRequest);
			ByteArrayToImage(imgDecryptResponse.Data);

			var test = ImageToByteArray(imgPath);
			ByteArrayToImage(test);

		}

		byte[] ImageToByteArray(string path)
		{
			return File.ReadAllBytes(path);
		}

		private void ByteArrayToImage(byte[] data)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				MemoryStream stream = new MemoryStream();
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				BitmapImage returnImage = new BitmapImage();
				returnImage.BeginInit();
				returnImage.CacheOption = BitmapCacheOption.OnLoad;
				returnImage.StreamSource = stream;
				returnImage.EndInit();
				returnImage.Freeze(); // Zorgt ervoor dat de afbeelding niet verandert nadat het is geladen

				Img.Source = returnImage;
			});
		}

	}
}