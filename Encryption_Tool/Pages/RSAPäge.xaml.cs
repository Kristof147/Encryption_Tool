using Encryption_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for RSAPäge.xaml
    /// </summary>
    public partial class RSAPäge : Page
    {
        private readonly FileManager? fm; 
        public string aESDirectoryPath;
        public RSAPäge()
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
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                aESDirectoryPath = selectedDirectory;

            }
        }
    }
}
