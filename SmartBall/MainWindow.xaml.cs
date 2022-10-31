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
using MaterialDesignThemes.Wpf;
namespace SmartBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Mode = false;
        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void PlayButtonClicked(object sender, RoutedEventArgs args)
        {
            ResultTextBox.Foreground = Brushes.LawnGreen;

            CommandExecutor cmdex = new CommandExecutor(Ruletka.textboxids, CodeTextBox.Text.Trim().Replace("\r\n", String.Empty), Ruletka.ballPos);

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
            //foreach (var pair in Ruletka.textboxids)
            //{
            //    if (((TextBox)pair.Key).Text == String.Empty)
            //    {
            //        ((TextBox)pair.Key).Background = Brushes.PaleVioletRed;

            //        return;
            //    }
            //}

            //foreach (var pair in Ruletka.textboxids)
            //{
            //    ((TextBox)pair.Key).Background = Brushes.White;
            //    Ruletka.Text[pair.Value] = ((TextBox)pair.Key).Text[0];
            //    ((TextBox)pair.Key).IsEnabled = EnterTextBox.IsEnabled;
            //}
            WordTextBox.Background = Brushes.White;
            
            if (Mode)
            { //ошибки режима кода
                if (WordTextBox.Text == String.Empty) 
                {
                    WordTextBox.Background = Brushes.PaleVioletRed;
                    return;
                }
                Ruletka.Text = new StringBuilder(String.Empty, 20);
                foreach (var pair in Ruletka.textboxids)
                    Ruletka.Text.Append(new StringBuilder(((TextBox)pair.Value).Text, 20));

                foreach (var letter in WordTextBox.Text)
                {
                    if (!(Ruletka.Text.ToString().Contains(letter)))
                    {
                        WordTextBox.Background = Brushes.PaleVioletRed;
                        return;
                    }
                }


                CodeTextBox.IsEnabled = !CodeTextBox.IsEnabled;
                WordTextBox.IsReadOnly = !WordTextBox.IsReadOnly;
            }
            else
            { //ошибки режима слова
                if (CodeTextBox.Text == String.Empty) //добавить брейкпоинты
                    return;


                CodeTextBox.IsReadOnly = !CodeTextBox.IsReadOnly;
                WordTextBox.IsEnabled = !WordTextBox.IsEnabled;
            }

            foreach (var pair in Ruletka.textboxids)
                ((TextBox)pair.Value).IsReadOnly = !((TextBox)pair.Value).IsReadOnly;

            PlayBtn.IsEnabled = !PlayBtn.IsEnabled;
            if (Icon.Kind == PackIconKind.Check)
            {
                Icon.Kind = PackIconKind.CancelOutline;
                RBtns.Visibility = Visibility.Hidden;
                Ruletka.RulerArea.Children.Remove(Ruletka.slider);
                Ruletka.RulerArea.ColumnDefinitions.Remove(Ruletka.Removal);
            }
            else
            {
                Icon.Kind = PackIconKind.Check;
                RBtns.Visibility = Visibility.Visible;
                Ruletka.RulerArea.Children.Add(Ruletka.slider);
                Ruletka.RulerArea.ColumnDefinitions.Add(Ruletka.Removal);
            }
        }

        private void RButtonChecked(object sender, RoutedEventArgs e)
        {
            CodeTextBox.IsEnabled = !CodeTextBox.IsEnabled;
            WordTextBox.IsEnabled = !WordTextBox.IsEnabled;
            Mode = !Mode;
        }
    }
}
