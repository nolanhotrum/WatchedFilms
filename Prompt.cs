using System.Windows;
using System.Windows.Controls;

namespace MoviesWatchedApp
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultValue = "")
        {
            Window prompt = new Window
            {
                Width = 500,
                Height = 150,
                Title = caption,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StackPanel stackPanel = new StackPanel();
            Label label = new Label { Content = text };
            TextBox textBox = new TextBox { Width = 400, Text = defaultValue };
            Button confirmation = new Button { Content = "Ok", Width = 100, IsDefault = true };
            confirmation.Click += (sender, e) => { prompt.DialogResult = true; prompt.Close(); };

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(confirmation);
            prompt.Content = stackPanel;
            prompt.ShowDialog();

            return textBox.Text;
        }
    }
}