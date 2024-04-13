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
        private readonly FileManager fm;
        string keyDirectoryPath = Properties.Settings.Default.KeyDirectoryPath;

        public KeysPage()
        {
            InitializeComponent();
        }
        

        private void BtnFolderKey_Click(object sender, RoutedEventArgs e)
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

        }

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
