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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for HashPage.xaml
    /// </summary>
    public partial class HashPage : Page
    {
        public HashPage()
        {
            InitializeComponent();
        }
        private void BtnHash_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; 
            openFileDialog.Multiselect = true; 


            if (openFileDialog.ShowDialog() == DialogResult.OK) 
            {
                
                if (openFileDialog.FileNames.Length == 2)
                {
                    string filePath1 = openFileDialog.FileNames[0];
                    string filePath2 = openFileDialog.FileNames[1];

                    string hash1 = HashingHelper.ComputeFileHash(filePath1);
                    string hash2 = HashingHelper.ComputeFileHash(filePath2);

                    string message = $"Hash of file 1:\n{hash1}\n\nHash of file 2:\n{hash2}";
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
