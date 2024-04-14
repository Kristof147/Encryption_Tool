using Encryption_Tool.Service;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for KeysPage.xaml
    /// </summary>
    public partial class KeysPage : Page
    {
        private Dictionary<string, Aes>? aesKeysDict;
        private readonly FileManager fm = new FileManager();
        string keyDirectoryPath = Properties.Settings.Default.KeyDirectoryPath;

        public KeysPage()
        {
            InitializeComponent();
        }

        private void BtnAES_Click(object sender, RoutedEventArgs e)
        {
            var input = Interaction.InputBox("Geef de naam van uw AES sleutel", "Aes Key Generate", "MyAesKey");
            if (!string.IsNullOrWhiteSpace(input))
            {
                KeyHelper.GenerateAESKey(keyDirectoryPath, input);
                //CmbAESKeys.Items.Clear();
                //InitializeKeys();
            }
        }
        private void BtnRSAPair_Click(object sender, RoutedEventArgs e)
        {
            // Vraag de gebruiker om een naam voor het sleutelpaar
            var input = Interaction.InputBox("Geef de naam van uw RSA sleutelpaar", "RSA Key Pair Generate", "MyRSAKeyPair");
            if (!string.IsNullOrWhiteSpace(input))
            {
                // Genereer het RSA-sleutelpaar
                KeyHelper.GenerateRSAKeyPair(keyDirectoryPath, input, out string publicKey, out string privateKey);
                // Toon een bericht aan de gebruiker dat de sleutels zijn gegenereerd
                System.Windows.MessageBox.Show($"RSA key pair gegenereerd en opgeslagen in de map {keyDirectoryPath}.", "RSA Key Pair Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //private void LoadKeys()
        //{
        //    // Controleer of de map voor sleutels bestaat
        //    if (!Directory.Exists(keyDirectoryPath))
        //    {
        //        Directory.CreateDirectory(keyDirectoryPath);
        //        return; // Stop als de map niet bestaat (er zijn geen sleutels om te laden)
        //    }

        //    // Laad AES-sleutels
        //    string[] aesKeyFiles = Directory.GetFiles(keyDirectoryPath, "*.aeskey");
        //    aesKeysDict = new Dictionary<string, Aes>();
        //    foreach (string filePath in aesKeyFiles)
        //    {
        //        string fileName = Path.GetFileNameWithoutExtension(filePath);
        //        byte[] keyAndIV = KeyHelper.DeserializeAes(filePath);
        //        if (keyAndIV != null && keyAndIV.Length >= 48) // Check of de lengte van de sleutel en IV correct is (32 bytes voor de sleutel en 16 bytes voor de IV)
        //        {
        //            byte[] key = new byte[32];
        //            byte[] iv = new byte[16];
        //            Array.Copy(keyAndIV, key, 32);
        //            Array.Copy(keyAndIV, 32, iv, 0, 16);
        //            aesKeysDict.Add(fileName, new AesCryptoServiceProvider() { Key = key, IV = iv });
        //        }
        //    }
        //}

        //private void SaveKeys()
        //{
        //    // Sla alleen AES-sleutels op, omdat RSA-sleutels al in XML-bestanden worden opgeslagen
        //    foreach (var entry in aesKeysDict)
        //    {
        //        string filePath = Path.Combine(keyDirectoryPath, entry.Key + ".aeskey");
        //        KeyHelper.SerializeAes(filePath, entry.Value.Key, entry.Value.IV);
        //    }
        //}

        private void BtnHash_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; // Allow all file types to be selected
            openFileDialog.Multiselect = true; // Allow multiple file selection


            if (openFileDialog.ShowDialog() == DialogResult.OK) // Check if dialog result is true (OK button clicked)
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

    }
}
