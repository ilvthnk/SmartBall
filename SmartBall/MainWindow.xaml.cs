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
            CommandExecutor cmdex = new CommandExecutor(Ruletka.Text.ToString().Trim(), EnterTextBox.Text.Trim().Replace("\r\n", String.Empty), Ruletka.ballPos);

            while (!cmdex.IsStopped)
            {
                cmdex.Next();

                Ruletka.setBallPos(cmdex.DataCursor);

                ResultTextBox.Text = cmdex.Result;
            }
        }
    }
}
