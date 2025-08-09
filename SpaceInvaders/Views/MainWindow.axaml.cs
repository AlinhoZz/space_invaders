using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SpaceInvaders.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            var gameWindow = new GameWindow();
            gameWindow.Show();
            
            this.Close();
        }
    }
}