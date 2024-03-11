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
		}
	}
}