﻿using Encryption_Tool.Service;
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

    }
}
