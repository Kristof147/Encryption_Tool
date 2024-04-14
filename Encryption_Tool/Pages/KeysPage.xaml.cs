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
                string publicKey, privateKey;
                // Genereer het RSA-sleutelpaar
                KeyHelper.GenerateRSAKeyPair(keyDirectoryPath, input, out publicKey, out privateKey);
                // Toon een bericht aan de gebruiker dat de sleutels zijn gegenereerd
                System.Windows.MessageBox.Show($"RSA key pair gegenereerd en opgeslagen in de map {keyDirectoryPath}.", "RSA Key Pair Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadKeys()
        {
            // Controleer of de map voor sleutels bestaat
            if (!Directory.Exists(keyDirectoryPath))
            {
                Directory.CreateDirectory(keyDirectoryPath);
                return; // Stop als de map niet bestaat (er zijn geen sleutels om te laden)
            }

            // Laad AES-sleutels
            string[] aesKeyFiles = Directory.GetFiles(keyDirectoryPath, "*.aeskey");
            aesKeysDict = new Dictionary<string, Aes>();
            foreach (string filePath in aesKeyFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                byte[] keyAndIV = KeyHelper.DeserializeAes(filePath);
                if (keyAndIV != null && keyAndIV.Length >= 48) // Check of de lengte van de sleutel en IV correct is (32 bytes voor de sleutel en 16 bytes voor de IV)
                {
                    byte[] key = new byte[32];
                    byte[] iv = new byte[16];
                    Array.Copy(keyAndIV, key, 32);
                    Array.Copy(keyAndIV, 32, iv, 0, 16);
                    aesKeysDict.Add(fileName, new AesCryptoServiceProvider() { Key = key, IV = iv });
                }
            }
        }

        private void SaveKeys()
        {
            // Sla alleen AES-sleutels op, omdat RSA-sleutels al in XML-bestanden worden opgeslagen
            foreach (var entry in aesKeysDict)
            {
                string filePath = Path.Combine(keyDirectoryPath, entry.Key + ".aeskey");
                KeyHelper.SerializeAes(filePath, entry.Value.Key, entry.Value.IV);
            }
        }

        

    }
}
