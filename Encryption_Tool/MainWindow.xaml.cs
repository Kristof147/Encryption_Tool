using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
using Microsoft.VisualBasic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace Encryption_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
        public static Window Owner;
        DispatcherTimer date = new DispatcherTimer();
		public Dictionary<string, Aes>? aesKeysDict;
        string aesImagePath = Properties.Settings.Default.AesImagePath;
        string aesTextPath = Properties.Settings.Default.AesTextPath;
        private readonly FileManager fm;
		string keyDirectoryPath = Properties.Settings.Default.KeyDirectoryPath;
		string aESDirectoryPath = Properties.Settings.Default.AesFolderPath;
		
		public MainWindow()
		{
			InitializeComponent();
			//InitializeKeys();
			fm = new FileManager();
            Owner = this;
            this.Width = 1000;
            this.Height = 600;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            EnableAndPrepareButtons(BtnHome);
            date.Interval = TimeSpan.FromSeconds(1);
            date.Tick += Date_Tick;
            date.Start();
            MainFrame.Content = new Pages.HomePage(); //Default

        }
        //private void InitializeKeys()
        //{
        //    aesKeysDict = new Dictionary<string, Aes>();


        //    if (Directory.Exists(keyDirectoryPath))
        //    {
        //        string[] files = Directory.GetFiles(keyDirectoryPath, "*.xml");

        //        foreach (string file in files)
        //        {
        //            string fileName = Path.GetFileName(file);
        //            if (fileName.Contains("_key.xml"))
        //            {
        //                Aes aes = Aes.Create();
        //                aes.Key = KeyHelper.DeserializeAes(file);
        //                string aesIv = file.Replace("_key.xml", "_iv.xml");
        //                aes.IV = KeyHelper.DeserializeAes(aesIv);
        //                aesKeysDict.Add(fileName, aes);
        //                CmbAESKeys.Items.Add(fileName.Replace("_key.xml", ""));
        //            }
        //        }
        //    }
        //    if (CmbAESKeys.Items.Count > 0)
        //        CmbAESKeys.SelectedIndex = 0;

        //}

				foreach (string file in files)
				{
					string fileName = Path.GetFileName(file);
					if (fileName.Contains(".xml"))
					{
						Aes aes = Aes.Create();
						aes.Key = KeyHelper.DeserializeAes(file);
						aesKeysDict.Add(fileName, aes);
						CmbAESKeys.Items.Add(fileName.Replace(".xml", ""));
					}
				}
			}
			if (CmbAESKeys.Items.Count > 0)
				CmbAESKeys.SelectedIndex = 0;

        private void Date_Tick(object sender, EventArgs e)
        {
            LblDate.Content = DateTime.Now.ToString("F");
        }
        private void EnableAndPrepareButtons(Button button)
        {
            foreach (var btn in SpMenu.Children)
            {
                if (btn is Button)
                {
                    (btn as Button).IsEnabled = true;
                    (btn as Button).BorderThickness = new Thickness(1, 0, 1, 1);
                }
            }

            button.IsEnabled = false;
            button.BorderThickness = new Thickness(1, 0, 0, 1);
        }

		#endregion

		#region AES tab
		// Encryption
		private void BtnAESEncrypt_Click(object sender, RoutedEventArgs e)
		{
			if (CmbAESKeys.SelectedIndex < 0)
				return;
			if (ChkAesImage.IsChecked == true && !File.Exists(aesImagePath))
				return;
			if (ChkAesText.IsChecked == true && string.IsNullOrWhiteSpace(TxtAes.Text))
				return;

			EncryptionEngine.CryptoEngine engine = new();
			CryptoParameters cryptoParameters = new() { Aes = aesKeysDict[$"{CmbAESKeys.SelectedItem}.xml"] };
			EncryptionRequest encryptRequest = new()
			{
				DataToEncrypt = ChkAesText.IsChecked == true ? Encoding.UTF8.GetBytes(TxtAes.Text) : ImageHelper.ImageToByteArray(aesImagePath),
				EncryptionType = EncryptionType.AES,
				Parameters = cryptoParameters
			};
			var response = engine.Encrypt(encryptRequest);
			if (response.Success)
			{
				var savePath = ChkAesText.IsChecked == true ? Properties.Settings.Default.AesTextPath : Properties.Settings.Default.AesImagePath;
				var result = Convert.ToBase64String(response.Data);

				Microsoft.Win32.SaveFileDialog sfd = new()
				{
					InitialDirectory = savePath,
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
					TxtAes.Text = string.Empty;
					ImageAes.Source = null;
				}
			}
			else
				MessageBox.Show("Er ging iets fout, probeer later opnieuw");
		}
		// Decryption
		private void BtnAESDecrypt_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TxtAes.Text))
				return;

			try
			{
				EncryptionEngine.CryptoEngine engine = new();
				Aes aes = Aes.Create();
				aes.Key = aesKeysDict[$"{CmbAESKeys.SelectedItem}.xml"].Key;
				CryptoParameters cryptoParameters = new() { Aes = aes };
				//string base64String = File.ReadAllText(ofd.FileName);
				string base64String = TxtAes.Text;
				byte[] dataToDecrypt = Convert.FromBase64String(base64String);

				DecryptionRequest decryptRequest = new()
				{
					DataToDecrypt = dataToDecrypt,
					EncryptionType = EncryptionType.AES,
					Parameters = cryptoParameters
				};
				var response = engine.Decryption(decryptRequest);
				if (response.Success)
				{
					if (ChkAesText.IsChecked == true)
					{
						TxtAes.Text = Encoding.UTF8.GetString(response.Data);
					}
					else
					{
						var image = ImageHelper.ByteArrayToImage(response.Data);
						ImageAes.Source = image;
						TxtAes.Text = string.Empty;
					}
				}
				else
				{
					MessageBox.Show($"Er ging iets fout tijdens de decryption!\nControleer als u de juiste key gebruikt.\nFoutcode:\n{response.Errors[0]}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Er is een fout opgetreden:\n" + ex.Message);
			}
		}

		// Image
		private void BtnAESImage_Click(object sender, RoutedEventArgs e)
		{
			aesImagePath = ImageHelper.LoadImage();
			if (!string.IsNullOrEmpty(aesImagePath))
			{
				ImageAes.Source = new BitmapImage(new Uri(aesImagePath));
			}
		}
		private void BtnAesSaveImage_Click(object sender, RoutedEventArgs e)
		{
			if (ImageAes.Source == null)
				return;

			Microsoft.Win32.SaveFileDialog sfd = new();
			sfd.Filter = "PNG Image|*.png";
			sfd.FileName = "DecryptedImages";
			sfd.OverwritePrompt = true;
			sfd.ValidateNames = true;
			Nullable<bool> result = sfd.ShowDialog();
			if (result == true)
			{
				var image = ImageAes.Source as BitmapSource;
				using (var fs = new FileStream(sfd.FileName, FileMode.Create))
				{
					BitmapEncoder encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(image));
					encoder.Save(fs);
				}
			}
		}
		// Load encrypted files
		private void BtnAesLoadText_Click(object sender, RoutedEventArgs e)
		{
			if (CmbAESKeys.SelectedIndex < 0)
				return;

			OpenFileDialog ofd = new();
			ofd.InitialDirectory = Properties.Settings.Default.AesTextPath;
			ofd.Filter = "Base64|*.b64";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TxtAes.Text = File.ReadAllText(ofd.FileName);
			}
		}

		private void BtnAesLoadImage_Click(object sender, RoutedEventArgs e)
		{
			if (CmbAESKeys.SelectedIndex < 0)
				return;

			OpenFileDialog ofd = new();
			ofd.InitialDirectory = Properties.Settings.Default.AesImagePath;
			ofd.Filter = "Base64|*.b64";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TxtAes.Text = File.ReadAllText(ofd.FileName);
			}
		}

		// Menu
		private void AesSetImagePathClick(object sender, RoutedEventArgs e)
		{
			string selectedDirectory = fm.SelectDirectory();
			if (!string.IsNullOrEmpty(selectedDirectory))
			{
				aesImagePath = selectedDirectory;
				Properties.Settings.Default.AesImagePath = aesImagePath;
				Properties.Settings.Default.Save();
			}
		}

		private void AesSetTextPathClick(object sender, RoutedEventArgs e)
		{
			string selectedDirectory = fm.SelectDirectory();
			if (!string.IsNullOrEmpty(selectedDirectory))
			{
				aesTextPath = selectedDirectory;
				Properties.Settings.Default.AesTextPath = aesTextPath;
				Properties.Settings.Default.Save();
			}
		}

		// Clear
		private void BtnAesClearText(object sender, RoutedEventArgs e)
		{
			TxtAes.Clear();
		}

		private void BtnAesClearImage(object sender, RoutedEventArgs e)
		{
			ImageAes.Source = null;
		}

		#endregion

		#region RSA tab
		private void BtnRSAEncrypt_Click(object sender, RoutedEventArgs e)
		{

		}

        private void BtnMenuKeys_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.KeysPage();
            EnableAndPrepareButtons((Button)sender);
        }

        private void BtnMenuAES_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.AESPage();
            EnableAndPrepareButtons((Button)sender);
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