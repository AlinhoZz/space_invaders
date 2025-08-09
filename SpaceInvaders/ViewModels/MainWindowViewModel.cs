using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SpaceInvaders.Views;

namespace SpaceInvaders.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand StartGameCommand { get; }

        public MainWindowViewModel()
        {
            StartGameCommand = new RelayCommand(ExecuteStartGame);
        }

        private void ExecuteStartGame()
        {
            var gameWindow = new GameWindow();
            
            gameWindow.Show();

            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                lifetime.MainWindow.Close();

            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute?.Invoke();

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
