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
using Button = System.Windows.Controls.Button;
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
        //            if (fileName.Contains(".xml"))
        //            {
        //                Aes aes = Aes.Create();
        //                aes.Key = KeyHelper.DeserializeAes(file);
        //                aesKeysDict.Add(fileName, aes);
        //                CmbAESKeys.Items.Add(fileName.Replace(".xml", ""));
        //            }
        //        }
        //    }
        //    if (CmbAESKeys.Items.Count > 0)
        //        CmbAESKeys.SelectedIndex = 0;

        //}

        private void Date_Tick(object sender, EventArgs e)
        {
            LblDate.Content = DateTime.Now.ToString("F");
        }
        private void EnableAndPrepareButtons(Button button)
        {
            foreach (var btn in SpMenu.Children)
            {
                if (btn is System.Windows.Controls.Button)
                {
                    (btn as Button).IsEnabled = true;
                    (btn as Button).BorderThickness = new Thickness(1, 0, 1, 1);
                }
            }

            button.IsEnabled = false;
            button.BorderThickness = new Thickness(1, 0, 0, 1);
        }

		


        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.HomePage();
            EnableAndPrepareButtons((Button)sender);
        }

        private void BtnMenuKeys_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.KeysPage();
            EnableAndPrepareButtons((Button)sender);
        }

        private void BtnMenuAES_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.AESPage();
            EnableAndPrepareButtons((Button)sender);
        }

        private void BtnMenuRSA_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.RSAPage();
            EnableAndPrepareButtons((Button)sender);
        }
        private void BtnMenuHash_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Pages.HashPage();
            EnableAndPrepareButtons((Button)sender);
        }

        private void MenuItemAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemFolderInstellen_Click(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                keyDirectoryPath = selectedDirectory;
                Properties.Settings.Default.KeyDirectoryPath = keyDirectoryPath;
                Properties.Settings.Default.Save();
                //InitializeKeys();
            }
        }
        private void MenuItemImagePath_Click(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                aesImagePath = selectedDirectory;
                Properties.Settings.Default.AesImagePath = aesImagePath;
                Properties.Settings.Default.Save();
            }
        }
        private void MenuItemTextPath_Click(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                aesTextPath = selectedDirectory;
                Properties.Settings.Default.AesTextPath = aesTextPath;
                Properties.Settings.Default.Save();
            }
        }

       
    }
}