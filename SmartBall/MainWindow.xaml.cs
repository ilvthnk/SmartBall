using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Text;
namespace SmartBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum AppMode
        {
            ModeCode,
            ModeGuess // Режим, в котором нужно определить какое слово выведет алгоритм
        }

        private AppMode Mode = AppMode.ModeCode;

        private DemoAppTask? task;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CancelCodeMode()
        {
            foreach (var pair in Ruler.RulerDelimeters)
                ((RulerDelimeter)pair.Value).TBox.IsEnabled = true;

            WordTextBox.IsEnabled = true;

            CodeTextBox.IsEnabled = false;

            PlayBtn.IsEnabled = false;

            ApplyBtn.Kind = PackIconKind.Check;

            RBtns.Visibility = Visibility.Visible;

            Ruler.RulerArea.Children.Add(Ruler.Slider);

            FileImportBtn.IsEnabled = false;

            Ruler.RulerArea.ColumnDefinitions.Add(Ruler.Removal);
        }

        private void ApplyCodeMode()
        {
            Mode = AppMode.ModeCode;

            WordTextBox.Foreground = Brushes.Black;

            ResultTextBox.Foreground = Brushes.Black;

            FileImportBtn.IsEnabled = false;

            // Ошибки режима кода
            if (WordTextBox.Text == String.Empty)
            {
                WordTextBox.Background = Brushes.PaleVioletRed;

                return;
            }

            Ruler.Text = new StringBuilder(String.Empty, 20);

            foreach (var pair in Ruler.RulerDelimeters)
            {
                // Все поля должны быть заполнены
                if (((RulerDelimeter)pair.Value).TBox.Text == String.Empty)
                {
                    ResultTextBox.Text = "Заполните линейку";

                    return;
                }
                else
                {
                    Ruler.Text.Append(((RulerDelimeter)pair.Value).TBox.Text[0]);
                }
            }

            foreach (var letter in WordTextBox.Text)
            {
                if (!(Ruler.Text.ToString().Contains(letter)))
                {
                    WordTextBox.Foreground = Brushes.PaleVioletRed;

                    return;
                }
            }

            foreach (var pair in Ruler.RulerDelimeters)
                ((RulerDelimeter)pair.Value).TBox.IsEnabled = false;

            WordTextBox.IsEnabled = false;

            CodeTextBox.IsEnabled = true;

            PlayBtn.IsEnabled = true;

            ApplyBtn.Kind = PackIconKind.CancelOutline;

            RBtns.Visibility = Visibility.Hidden;

            Ruler.RulerArea.Children.Remove(Ruler.Slider);

            Ruler.RulerArea.ColumnDefinitions.Remove(Ruler.Removal);
        }

        private void CancelGuessMode()
        {
            Mode = AppMode.ModeCode;

            CheckCodeBth.IsChecked = true;

            foreach (var pair in Ruler.RulerDelimeters)
                ((RulerDelimeter)pair.Value).TBox.IsEnabled = true;

            WordTextBox.IsEnabled = true;

            ResultTextBox.IsReadOnly = true;

            PlayBtn.IsEnabled = false;

            ApplyBtn.Kind = PackIconKind.Check;

            RBtns.Visibility = Visibility.Visible;

            FileImportBtn.IsEnabled = false;

            Ruler.RulerArea.Children.Add(Ruler.Slider);

            Ruler.RulerArea.ColumnDefinitions.Add(Ruler.Removal);
        }

        private void ApplyGuessMode()
        {
            Mode = AppMode.ModeGuess;

            foreach (var pair in Ruler.RulerDelimeters)
                ((RulerDelimeter)pair.Value).TBox.IsEnabled = false;

            WordTextBox.IsEnabled = false;

            CodeTextBox.IsEnabled = false;

            ResultTextBox.IsReadOnly = false;

            PlayBtn.IsEnabled = false;

            FileImportBtn.IsEnabled = true;

            ApplyBtn.Kind = PackIconKind.CancelOutline;

            RBtns.Visibility = Visibility.Hidden;

            Ruler.RulerArea.Children.Remove(Ruler.Slider);

            Ruler.RulerArea.ColumnDefinitions.Remove(Ruler.Removal);
        }

        private void FileImportButtonClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    string filename = dialog.FileName;

                    task = JsonSerializer.Deserialize<DemoAppTask>(File.ReadAllText(filename));

                    WordTextBox.Text = task.Task;

                    CodeTextBox.Text = task.Code;

                    Ruler.SetBallPos(task.BallPos);

                    Ruler.Text = new StringBuilder(String.Empty, 20);

                    if (task.RulerData.Length > Ruler.Size)
                    {
                        for (int i = 0; i < task.RulerData.Length - Ruler.Size; i++)
                            Ruler.AppendDelimeter();
                    }
                    else if (task.RulerData.Length < Ruler.Size)
                    {
                        for (int i = 0; i < Ruler.Size - task.RulerData.Length; i++)
                            Ruler.RemoveLast();
                    }

                    for (int i = 0; i < task.RulerData.Length; i++)
                    {
                        Ruler.RulerDelimeters[i].TBox.Text = task.RulerData[i].ToString();
                        Ruler.Text.Append(task.RulerData[i]);
                    }
                }

                PlayBtn.IsEnabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void PlayButtonClicked(object sender, RoutedEventArgs args)
        {
            CommandExecutor cmdex = new CommandExecutor(Ruler.RulerDelimeters, CodeTextBox.Text.Trim().Replace("\r\n", String.Empty), Ruler.BallPos);

            try
            {
                while (!cmdex.IsStopped)
                {
                    cmdex.Next();

                    Ruler.SetBallPos(cmdex.DataCursor);

                    if (Mode != AppMode.ModeGuess)
                        ResultTextBox.Text = cmdex.Result;
                }

                if (Mode == AppMode.ModeGuess)
                {
                    if (ResultTextBox.Text != task.Answer)
                    {
                        ResultTextBox.Foreground = Brushes.PaleVioletRed;

                        Ruler.SetBallPos(task.BallPos);
                    }
                    else
                        ResultTextBox.Foreground = Brushes.LawnGreen;
                }
            }
            catch (Exception e)
            {
                ResultTextBox.Text = e.Message;
            }
        }

        private void RBtnCodeClicked(object sender, RoutedEventArgs args)
        {
            if (Mode != AppMode.ModeCode)
                ApplyCodeMode();
        }

        private void RBtnGuessClicked(object sender, RoutedEventArgs args)
        {
            if (Mode != AppMode.ModeGuess)
                ApplyGuessMode();
        }

        private void CheckButtonClicked(object sender, RoutedEventArgs args)
        {
            if (ApplyBtn.Kind == PackIconKind.Check && Mode == AppMode.ModeCode)
                ApplyCodeMode();
            else if (ApplyBtn.Kind == PackIconKind.Check && Mode == AppMode.ModeGuess)
                ApplyGuessMode();
            else if (ApplyBtn.Kind == PackIconKind.CancelOutline && Mode == AppMode.ModeCode)
                CancelCodeMode();
            else if (ApplyBtn.Kind == PackIconKind.CancelOutline && Mode == AppMode.ModeGuess)
                CancelGuessMode();
        }
    }
}
