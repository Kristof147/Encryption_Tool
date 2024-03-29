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

namespace Encryption_Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private Dictionary<string, string>? aesKeysDict;
        public MainWindow()
		{
			InitializeComponent();
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

            // Specify the directory path where your files are stored
            string directoryPath = @"C:\Your\Directory\Path";

            if (Directory.Exists(directoryPath))
            {
                // Get all files with a specific extension, e.g., .txt
                string[] files = Directory.GetFiles(directoryPath, "*.txt");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string base64Key = File.ReadAllText(file).Trim(); // Assuming the key is stored in the file

                    aesKeysDict.Add(fileName, base64Key);
                    CmbAESKeys.Items.Add(fileName); // Add file names to ComboBox
                }
            }
            else
            {
                MessageBox.Show("Directory not found.");
            }
        }
    }
}