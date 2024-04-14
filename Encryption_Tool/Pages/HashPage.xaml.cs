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
            OpenFileDialog openFileDialog = new()
            {
                Filter = "All Files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            if(openFileDialog.FileNames.Length != 2)
            {
                System.Windows.MessageBox.Show("Please select exactly two files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (openFileDialog.FileNames.Length == 2)
            {
                string hash1 = HashingHelper.ComputeFileHash(openFileDialog.FileNames[0]);
                string hash2 = HashingHelper.ComputeFileHash(openFileDialog.FileNames[1]);

                string message = $"Hash of file 1:\n{hash1}\n\nHash of file 2:\n{hash2}\n\nThe hashes are {(hash1 == hash2 ? "the same" : "different")}";
                System.Windows.MessageBox.Show(message, "Hash Comparison Result using SHA256", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
