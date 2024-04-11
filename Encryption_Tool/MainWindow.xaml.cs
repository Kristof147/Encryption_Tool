using Encryption_Tool.Crypto.Cryptors;
using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
using Microsoft.VisualBasic;
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
using MessageBox = System.Windows.MessageBox;

namespace Encryption_Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private Dictionary<string, Aes>? aesKeysDict;
		private readonly FileManager fm;
		string keyDirectoryPath = Properties.Settings.Default.KeyDirectoryPath;
		string imageDirectoryPath = @"C:\test";
		string aESDirectoryPath = Properties.Settings.Default.AesFolderPath;
		string aesImagePath = Properties.Settings.Default.AesImagePath;
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
			aesKeysDict = new Dictionary<string, Aes>();


			if (Directory.Exists(keyDirectoryPath))
			{
				string[] files = Directory.GetFiles(keyDirectoryPath, "*.xml");

				foreach (string file in files)
				{
					string fileName = Path.GetFileName(file);
					if (fileName.Contains("_key.xml"))
					{
						Aes aes = Aes.Create();
						aes.Key = KeyHelper.DeserializeAes(file);
						string aesIv = file.Replace("_key.xml", "_iv.xml");
						aes.IV = KeyHelper.DeserializeAes(aesIv);
						aesKeysDict.Add(fileName, aes);
						CmbAESKeys.Items.Add(fileName.Replace("_key.xml", ""));
					}
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
				Properties.Settings.Default.KeyDirectoryPath = keyDirectoryPath;
				Properties.Settings.Default.Save();
				InitializeKeys();
			}
		}

		private void BtnAES_Click(object sender, RoutedEventArgs e)
		{
			var input = Interaction.InputBox("Geef de naam van uw AES sleutel", "Aes Key Generate", "MyAesKey");
			if (!string.IsNullOrWhiteSpace(input))
			{
				KeyHelper.GenerateAESKey(keyDirectoryPath, input);
				CmbAESKeys.Items.Clear();
				InitializeKeys();
			}
		}
		private void BtnRSAPair_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnHash_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "All Files (*.*)|*.*"; // Allow all file types to be selected
			openFileDialog.Multiselect = true; // Allow multiple file selection


			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // Check if dialog result is true (OK button clicked)
			{
				// Check if exactly two files are selected
				if (openFileDialog.FileNames.Length == 2)
				{
					// Calculate the hashes of the selected files
					string filePath1 = openFileDialog.FileNames[0];
					string filePath2 = openFileDialog.FileNames[1];

					string hash1 = HashingHelper.ComputeFileHash(filePath1);
					string hash2 = HashingHelper.ComputeFileHash(filePath2);

					// Display the hashes in a MessageBox
					string message = $"Hash of file 1:\n{hash1}\n\nHash of file 2:\n{hash2}";

					// Compare the hashes and display the result
					if (hash1 == hash2)
					{
						message += "\n\nThe hashes are the same.";
					}
					else
					{
						message += "\n\nThe hashes are different.";
					}

					System.Windows.MessageBox.Show(message, "Hash Comparison Result", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					System.Windows.MessageBox.Show("Please select exactly two files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
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
			if (CmbAESKeys.SelectedIndex < 0)
				return;

			EncryptionEngine.CryptoEngine engine = new();
			CryptoParameters cryptoParameters = new() { Aes = aesKeysDict[$"{CmbAESKeys.SelectedItem}_key.xml"] };
			EncryptionRequest encryptRequest = new()
			{
				DataToEncrypt = ImageHelper.ImageToByteArray(aesImagePath),
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			var response = engine.Encrypt(encryptRequest);
			if (response.Success)
			{
				var result = Convert.ToBase64String(response.Data);

				Microsoft.Win32.SaveFileDialog sfd = new()
				{
					InitialDirectory = imageDirectoryPath,
					Filter = "Base64|*.b64",
					FileName = "EncryptionName",
					DefaultExt = "b64",
					OverwritePrompt = true,
					ValidateNames = true
				};
				Nullable<bool> sfdResult = sfd.ShowDialog();
				if (sfdResult == true)
				{
					byte[] bytesToWrite = Encoding.UTF8.GetBytes(result);
					File.WriteAllBytes(sfd.FileName, bytesToWrite);
				}
			}
			else
				MessageBox.Show("Er ging iets fout, probeer later opnieuw");
		}

		private void BtnAESDecrypt_Click(object sender, RoutedEventArgs e)
		{
			if (CmbAESKeys.SelectedIndex < 0)
				return;

			OpenFileDialog ofd = new();
			ofd.InitialDirectory = imageDirectoryPath;
			ofd.Filter = "Base64|*.b64";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				EncryptionEngine.CryptoEngine engine = new();
				CryptoParameters cryptoParameters = new() { Aes = aesKeysDict[$"{CmbAESKeys.SelectedItem}_key.xml"] };
				string base64String = File.ReadAllText(ofd.FileName);
				byte[] dataToDecrypt = Convert.FromBase64String(base64String);

				DecryptionRequest decryptRequest = new()
				{
					DataToDecrypt = dataToDecrypt,//ImageHelper.ImageToByteArray(ofd.FileName),
					EncryptionType = EncryptionType.AES,
					Parameters = cryptoParameters
				};
				var response = engine.Decryption(decryptRequest);
				if (response.Success)
				{
					var image = ImageHelper.ByteArrayToImage(response.Data);
					ImageAes.Source = image;
				}
				else
				{
					MessageBox.Show($"Er ging iets fout tijdens de decryption!\nControleer als u de juiste key gebruikt.\nFoutcode:\n{response.Errors[0]}");
				}
			}
		}

		private void BtnAESImage_Click(object sender, RoutedEventArgs e)
		{
			aesImagePath = ImageHelper.LoadImage();
			if (!string.IsNullOrEmpty(aesImagePath))
			{
				ImageAes.Source = new BitmapImage(new Uri(aesImagePath));
			}
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



	}
}