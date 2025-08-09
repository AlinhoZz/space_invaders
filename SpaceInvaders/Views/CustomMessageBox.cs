using Avalonia.Controls;
using Avalonia.Layout;

namespace SpaceInvaders.Views
{
    public class CustomMessageBox : Window
    {
        public CustomMessageBox(string message)
        {

            var textBlock = new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var button = new Button
            {
                Content = "Ok",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            button.Click += (s, e) => Close();

            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);

            Content = stackPanel;

            Width = 250;
            Height = 150;
        }
    }
}