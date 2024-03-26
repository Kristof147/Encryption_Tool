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
		}

        private void btnKeyGeneration_Click(object sender, RoutedEventArgs e)
        {
            string publicKey;
            string privateKey;

            KeyHelper.SaveKeys(out publicKey, out privateKey);
            txtPublicKey.Text = publicKey;
            txtPrivateKey.Text = privateKey;
        }
    }
}