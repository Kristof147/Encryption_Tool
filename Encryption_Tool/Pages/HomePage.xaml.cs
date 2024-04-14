using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Encryption_Tool.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page ,INotifyPropertyChanged
    {
        private string[] _welcomeMessages = ["Welkom bij de Encryption Tool!", "Van Seppe, Kristof, Benjamin en Islambek", "Kies uw optie links om te beginnen!"];
        private int _currentIndex = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public string WelcomeText
        {
            get { return _welcomeMessages[_currentIndex]; }
        }

        public HomePage()
        {
            InitializeComponent();
            DataContext = this;

            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _currentIndex = (_currentIndex + 1) % _welcomeMessages.Length;
            OnPropertyChanged(nameof(WelcomeText));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
