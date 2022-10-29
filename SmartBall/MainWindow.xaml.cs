using System;
using System.Collections.Generic;
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
using SmartBall.UserControls;
namespace SmartBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void PlayButtonClicked(object sender, RoutedEventArgs args)
        {
            ResultTextBox.Foreground = Brushes.LawnGreen;

            CommandExecutor cmdex = new CommandExecutor(Ruletka.Text.ToString().Trim(), EnterTextBox.Text.Trim().Replace("\r\n", String.Empty), Ruletka.ballPos);

            try
            {
                while (!cmdex.IsStopped)
                {
                    cmdex.Next();

                    Ruletka.setBallPos(cmdex.DataCursor);

                    ResultTextBox.Text = cmdex.Result;
                }
            }
            catch (Exception e)
            {
                ResultTextBox.Foreground = Brushes.PaleVioletRed;
                ResultTextBox.Text = e.Message;
            }
        }

        private void CheckButtonClicked(object sender, RoutedEventArgs args)
        {
            foreach (var pair in Ruletka.textboxids)
            {
                if (((TextBox)pair.Key).Text == String.Empty)
                {
                    ((TextBox)pair.Key).Background = Brushes.PaleVioletRed;

                    return;
                }
            }

            foreach (var pair in Ruletka.textboxids)
            {
                ((TextBox)pair.Key).Background = Brushes.White;
                Ruletka.Text[pair.Value] = ((TextBox)pair.Key).Text[0];
                ((TextBox)pair.Key).IsEnabled = EnterTextBox.IsEnabled;
            }

            EnterTextBox.IsEnabled = !EnterTextBox.IsEnabled;
            PlayBtn.IsEnabled = EnterTextBox.IsEnabled;
        }
    }
}
