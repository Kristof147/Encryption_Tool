using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Brushes = System.Windows.Media.Brushes;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for RSAPage.xaml
    /// </summary>
    public partial class RSAPage : Page
    {
        private Dictionary<string, RSA>? rsaPubKeys;
        private Dictionary<string, RSA>? rsaPrivKeys;
        private Dictionary<string, byte[]>? aesKeys;
        private List<string> encryptedAesKeys;
        private readonly FileManager fm = new();
        string keyDirectoryPath = Properties.Settings.Default.KeyDirectoryPath;
        string rSADirectoryPath = Properties.Settings.Default.RsaFolderPath;
        string defaultEncryptedPath = Properties.Settings.Default.EncryptedAesKeyPath;
        readonly EncryptionEngine.CryptoEngine engine = new();


        public RSAPage()
        {
            InitializeComponent();
            InitializeKeys();
        }


        private void InitializeKeys()
        {
            rsaPubKeys = [];
            rsaPrivKeys = [];
            aesKeys = [];
            encryptedAesKeys = [];
            DirectoryInfo info = new(keyDirectoryPath);
            DirectoryInfo encryptedpath = new(defaultEncryptedPath);
            FileInfo[] allKeys = [.. info.GetFiles("*.xml").Concat(info.GetFiles("*.b64"))];
            if (!encryptedpath.Equals(info))
                allKeys = [.. allKeys.Concat(encryptedpath.GetFiles("*.xml").Concat(encryptedpath.GetFiles("*.b64")))];
            if (Directory.Exists(keyDirectoryPath))
            {
                //Gets all the xml files in the directory
                foreach (FileInfo file in allKeys)
                {
                    using var reader = file.OpenText();
                    var firstLine = reader.ReadLine();

                    //Check if the file is a RSA key
                    if (firstLine?.StartsWith("<RSAKeyValue>") ?? false)
                    {
                        reader.Dispose();
                        RSA rsa = RSA.Create();
                        rsa.FromXmlString(File.ReadAllText(file.FullName));
                        if (file.Name.EndsWith("_private.xml"))
                            rsaPrivKeys.TryAdd(file.Name.Replace("_private.xml", "(private)"), rsa);
                        else
                            rsaPubKeys.TryAdd(file.Name.Replace("_public.xml", "(public)"), rsa);
                    }
                    else if (firstLine?.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>") ?? false)
                    {
                        reader.Dispose();
                        aesKeys.TryAdd(file.Name.Replace(".xml", ""), KeyHelper.DeserializeAes(file.FullName));
                    }
                    else if (firstLine?.StartsWith("<EncryptedAES>") ?? false)
                    {
                        encryptedAesKeys.Add(file.Name.Replace("_enc.b64", " (encrypted)"));
                        aesKeys.TryAdd(file.Name.Replace("_enc.b64", " (encrypted)"), Convert.FromBase64String(reader.ReadToEnd()));
                    }
                }
                LstBoxKeys.ItemsSource = rsaPubKeys.Concat(rsaPrivKeys);
                //CmbRSAPriKey.ItemsSource = rsaPrivKeys;
                LstBoxAesKeys.ItemsSource = aesKeys;

                LstBoxKeys.DisplayMemberPath = LstBoxAesKeys.DisplayMemberPath = "Key";
                LstBoxKeys.SelectedValuePath = LstBoxAesKeys.SelectedValuePath = "Value";
            }
        }
        private void BtnRSAEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var pair = (KeyValuePair<string, byte[]>)LstBoxAesKeys.SelectedItem;
            if (LstBoxAesKeys.SelectedIndex < 0)
            {
                MessageBox.Show("Selecteer een AES sleutel om te encrypteren", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (LstBoxAesKeys.SelectedIndex < 0 ||
                LstBoxKeys.SelectedValue is not RSA selectedKey)
            {
                MessageBox.Show("Selecteer een publieke sleutel om te encrypteren", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            EncryptionRequest request = new()
            {
                DataToEncrypt = (byte[])LstBoxAesKeys.SelectedValue,
                EncryptionType = EncryptionType.RSA,
                Parameters = new CryptoParameters() { RSA = selectedKey }
            };
            var response = engine.Encrypt(request);
            if (!response.Success)
            {
                MessageBox.Show("Er is iets fout gegaan bij het encrypteren van de AES sleutel", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            SaveFileDialog sfd = new()
            {
                InitialDirectory = defaultEncryptedPath,
                Filter = "(Base 64)|*_enc.b64",
                FileName = $"{pair.Key.Replace("public", "")}_enc.b64"
            };
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("het encrypteren van de sleutel is gestopt", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string encoded = "<EncryptedAES>\n" + Convert.ToBase64String(response.Data);
            File.WriteAllText(sfd.FileName, encoded);
            MessageBox.Show("AES sleutel is geencrypteerd", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private void BtnRSADecrypt_Click(object sender, RoutedEventArgs e)
        {
            var pair = (KeyValuePair<string, byte[]>)LstBoxAesKeys.SelectedItem;
            if (LstBoxAesKeys.SelectedIndex < 0)
            {
                MessageBox.Show("Selecteer een AES sleutel om te decrypteren", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (LstBoxKeys.SelectedIndex < 0 ||
                LstBoxKeys.SelectedValue is not RSA selectedKey)
            {
                MessageBox.Show("Selecteer een private sleutel om te decryptere", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DecryptionRequest request = new()
            {
                DataToDecrypt = (byte[])LstBoxAesKeys.SelectedValue,
                EncryptionType = EncryptionType.RSA,
                Parameters = new CryptoParameters() { RSA = selectedKey }
            };
            var response = engine.Decryption(request);

            if (!response.Success)
            {
                MessageBox.Show($"Er is iets fout gegaan bij het encrypteren van de AES sleutel: {response.Errors[0]}", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SaveFileDialog sfd = new()
            {
                InitialDirectory = keyDirectoryPath,
                Filter = "(*.xml)|*.xml",
                FileName = $"{pair.Key.Replace(" (encrypted)", "")}.xml"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Aes temp = Aes.Create();
                temp.Key = response.Data;
                KeyHelper.SaveAesKey(sfd.FileName, temp);
                MessageBox.Show($"AES sleutel is gedecrypteerd\n\n file hash: {HashingHelper.ComputeFileHash(sfd.FileName)}", "RSA Encryptie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRSAFolder_Click(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = fm.SelectDirectory();
            if (!string.IsNullOrEmpty(selectedDirectory))
            {
                defaultEncryptedPath = selectedDirectory;
                Properties.Settings.Default.EncryptedAesKeyPath = selectedDirectory;
                Properties.Settings.Default.Save();
            }
        }

        private void LstAesKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstBoxAesKeys.SelectedItem is not KeyValuePair<string, byte[]> selectedKey)
                return;

            if (encryptedAesKeys.Contains(selectedKey.Key))
            {
                BtnRSADecrypt.IsEnabled = true;
                BtnRSAEncrypt.IsEnabled = false;
                BtnRSADecrypt.ToolTip = null;
                BtnRSAEncrypt.ToolTip = "Deze sleutel is al geencrypteerd";

            }
            else
            {
                BtnRSADecrypt.IsEnabled = false;
                BtnRSAEncrypt.IsEnabled = true;
                BtnRSAEncrypt.ToolTip = null;
                BtnRSADecrypt.ToolTip = "Deze sleutel is nog niet geencrypteerd";
            }
        }
        private void FilterChecked(object sender, RoutedEventArgs e)
        {

            if (sender is not CheckBox checkBox ||aesKeys is null)
                return;
            ToggelLabelState(checkBox);
        }

        private void ToggelLabelState(CheckBox checkbox)
        {
            if (ChkBoxPlain is not null && ChkBoxEncrypted is not null)
            {
                if ((ChkBoxPlain.IsChecked??false) && (ChkBoxEncrypted.IsChecked??false))
                    ToggleFilter(FilterType.All);
                else if (ChkBoxPlain.IsChecked??false)
                    ToggleFilter(FilterType.NotEncrypted);
                else if (ChkBoxEncrypted.IsChecked??false)
                    ToggleFilter(FilterType.Encrypted);
                
            }
            bool isChecked = checkbox.IsChecked ?? false;
            checkbox.Foreground = isChecked? Brushes.Green : Brushes.Black;
            checkbox.FontWeight = isChecked ? FontWeights.Bold : FontWeights.Normal;
            checkbox.Opacity =isChecked? 1 :  0.5;
        }
        private void ToggleFilter(FilterType type)
        {
            switch(type)
            {
                case FilterType.All:
                    LstBoxAesKeys.ItemsSource = aesKeys;
                    LstBoxKeys.ItemsSource = rsaPubKeys.Concat(rsaPrivKeys);
                    break;
                case FilterType.Encrypted:
                    LstBoxAesKeys.ItemsSource = aesKeys.Where((keyPair) => encryptedAesKeys.Contains(keyPair.Key));
                    LstBoxKeys.ItemsSource =rsaPrivKeys;
                    break;
                case FilterType.NotEncrypted:
                    LstBoxAesKeys.ItemsSource = aesKeys.Where((keyPair) => !encryptedAesKeys.Contains(keyPair.Key));
                    LstBoxKeys.ItemsSource = rsaPubKeys;
                    break;
            }

        }
    }
    public enum FilterType
    {
        All,
        Encrypted,
        NotEncrypted
    }
}
