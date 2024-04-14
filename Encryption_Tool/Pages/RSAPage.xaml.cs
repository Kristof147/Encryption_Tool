using Encryption_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for RSAPage.xaml
    /// </summary>
    public partial class RSAPage : Page
    {
        private Dictionary<string, Aes>? aesKeysDict;
        //private readonly FileManager fm;
        string rSADirectoryPath = Properties.Settings.Default.RsaFolderPath;
     
        public RSAPage()
        {
            InitializeComponent();
        }

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
            FileManager fm = new FileManager();
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                rSADirectoryPath = selectedDirectory;
                
            }
            else
            {
                MessageBox.Show("Standard folder not changed");
            }
        }
    }
}
