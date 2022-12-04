using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SmartBall.UserControls;
using MaterialDesignThemes.Wpf;
using System.Text.Json;

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
            ModeGuess
        }

        private string RightAnswer = ""; // Для режима "слово"

        private AppMode Mode = AppMode.ModeCode; // По умолчанию

        public MainWindow()
        {
            InitializeComponent();
        }

        // Для загрузки файла
        private void FileImportButtonClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

                bool? result = dialog.ShowDialog();

                DemoAppTask task = new DemoAppTask();

                if (result == true)
                {
                    string fileName = dialog.FileName;

                    task = JsonSerializer.Deserialize<DemoAppTask>(File.ReadAllText(fileName));

                    if (task == null)
                        throw new Exception("Ошибка чтения файла!");

                    if (task.RulerData == null)
                        throw new Exception("Линейка должна быть заполнена!");

                    task.RulerData = task.RulerData.Trim().Replace("\r\n", string.Empty).Replace(" ", string.Empty);

                    if (task.RulerData.Length < 4 || task.RulerData.Length > 20)
                        throw new Exception("На линейке должно быть от 4 и до 20 символов!");

                    Ruler.Slider.Value = task.RulerData.Length;

                    if (task.BallPos < 0 || task.BallPos >= task.RulerData.Length)
                        task.BallPos = 0;

                    if (Ruler.BallPos >= task.RulerData.Length)
                        Ruler.BallPos = task.RulerData.Length - 1;

                    Ruler.SetBallPos(task.BallPos);

                    for (int i = 0; i < task.RulerData.Length; i++)
                    {
                        Ruler.RulerDelimeters[i].TBox.Text = task.RulerData[i].ToString();

                        Ruler.Text.Append(task.RulerData[i]);
                    }

                    WordTextBox.Text = task.Task;

                    ResultTextBox.Foreground = Brushes.Black;

                    if (task.Code == string.Empty)
                    {
                        Mode = AppMode.ModeCode;

                        WordTextBox.IsReadOnly = false;

                        ResultTextBox.IsReadOnly = true;

                        PlayBtn.IsEnabled = false;

                        CodeTextBox.IsEnabled = false;

                        ApplyBtn.Kind = PackIconKind.Check;

                        CheckCodeBtn.IsChecked = true;
                    }
                    else
                    {
                        Mode = AppMode.ModeGuess;
                        Mode = AppMode.ModeGuess;

                        WordTextBox.IsReadOnly = true;

                        ResultTextBox.IsReadOnly = false;

                        PlayBtn.IsEnabled = true;

                        ApplyBtn.Kind = PackIconKind.CancelOutline;

                        RBtns.Visibility = Visibility.Hidden;

                        CodeTextBox.IsEnabled = false;
                        CodeTextBox.Text = task.Code;

                        CheckGuessBtn.IsChecked = true;

                        foreach (var pair in Ruler.RulerDelimeters)
                        {
                            (pair.Value).TBox.IsReadOnly = true;
                        }

                        Ruler.RulerArea.Children.Remove(Ruler.Slider);

                        Ruler.RulerArea.ColumnDefinitions.Remove(Ruler.Removal);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        // Запуск в данном режиме
        private void PlayButtonClicked(object sender, RoutedEventArgs args)
        {
            CommandExecutor cmdex = new CommandExecutor(Ruler.RulerDelimeters, CodeTextBox.Text.Trim().Replace("\r\n", string.Empty), Ruler.BallPos);

            try
            {
                string UserAnswer = ResultTextBox.Text;

                int SavedBallPos = Ruler.BallPos;

                if (CodeTextBox.Text == String.Empty) throw new Exception("Введи алгоритм!");

                while (!cmdex.IsStopped)
                {
                    cmdex.Next();

                    Ruler.SetBallPos(cmdex.DataCursor);
                }

                RightAnswer = cmdex.Result;

                if (Mode == AppMode.ModeCode)
                {
                    ResultTextBox.Text = cmdex.Result;
                }

                if (Mode == AppMode.ModeGuess)
                {
                    if (UserAnswer == RightAnswer)
                    {
                        ResultTextBox.Foreground = Brushes.LawnGreen;
                    }
                    else
                    {
                        ResultTextBox.Foreground = Brushes.DarkRed;

                        Ruler.SetBallPos(SavedBallPos);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "Умный шарик");
            }
        }

        // Переключение в режим "код"
        private void RBtnCodeClicked(object sender, RoutedEventArgs args)
        {
            WordTextBox.IsReadOnly = false;

            ResultTextBox.Foreground = Brushes.Black;

            if (Mode != AppMode.ModeCode)
            {
                Mode = AppMode.ModeCode;

                ResultTextBox.IsReadOnly = true;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = false;
            }
        }

        // Переключение в режим "слово"
        private void RBtnGuessClicked(object sender, RoutedEventArgs args)
        {
            WordTextBox.IsReadOnly = false;

            ResultTextBox.Foreground = Brushes.Black;

            if (Mode != AppMode.ModeGuess)
            {
                Mode = AppMode.ModeGuess;

                ResultTextBox.IsReadOnly = true;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = true;
            }
        }

        // Применение или возврат в редактирование (подготовка к запуску)
        private void CheckButtonClicked(object sender, RoutedEventArgs args)
        {
            if (ApplyBtn.Kind == PackIconKind.Check)
                Apply();
            else if (ApplyBtn.Kind == PackIconKind.CancelOutline)
                Cancel();
        }

        // Применение изменений
        private void Apply()
        {
            /*
             * В обоих режимах линейка должна быть заполнена, а слайдер должен удалиться. Поля линейки нельзя изменять.
             */

            foreach (var pair in Ruler.RulerDelimeters)
            {
                if ((pair.Value).TBox.Text == string.Empty)
                {
                    MessageBox.Show(this, "Заполни линейку!", "Не даром я умный!");

                    return;
                }
                else
                {
                    Ruler.Text.Append((pair.Value).TBox.Text[0]);

                    (pair.Value).TBox.IsReadOnly = true;
                }
            }

            ResultTextBox.Foreground = Brushes.Black;

            WordTextBox.IsReadOnly = true;

            Ruler.Text = new StringBuilder(string.Empty, 20);

            RBtns.Visibility = Visibility.Hidden;

            Ruler.RulerArea.Children.Remove(Ruler.Slider);

            Ruler.RulerArea.ColumnDefinitions.Remove(Ruler.Removal);

            /* В режиме "код":
            * - поле для ввода слова становится доступным только для чтения
            * - кнопка запуска становится активной
            * - кнопка применения меняется на кнопкук отмены
            * - текстовое поле становится активным
            */

            if (Mode == AppMode.ModeCode)
            {
                ResultTextBox.Text = "";

                ResultTextBox.IsReadOnly = true;

                PlayBtn.IsEnabled = true;

                ApplyBtn.Kind = PackIconKind.CancelOutline;

                CodeTextBox.IsEnabled = true;
            }

            /* В режиме "слово":
            * - поле для ввода слова становится доступным для редактирования
            * - кнопка запуска становится активной
            * - кнопка применения меняется на кнопкук отмены
            * - текстовое поле становится неактивным
            */

            if (Mode == AppMode.ModeGuess)
            {
                ResultTextBox.IsReadOnly = false;

                PlayBtn.IsEnabled = true;

                ApplyBtn.Kind = PackIconKind.CancelOutline;

                CodeTextBox.IsEnabled = false;
            }
        }

        // Откат в режим редактирования (ничего не сохраняется)
        private void Cancel()
        {
            WordTextBox.IsReadOnly = false;

            RBtns.Visibility = Visibility.Visible;

            ResultTextBox.Foreground = Brushes.Black;

            /*
             * В обоих режимах возвращаем слайдер. Поля линейки можно изменять.
             */

            foreach (var pair in Ruler.RulerDelimeters)
            {
                (pair.Value).TBox.IsReadOnly = false;
            }

            Ruler.RulerArea.Children.Add(Ruler.Slider);

            Ruler.RulerArea.ColumnDefinitions.Add(Ruler.Removal);

            /* В режиме "код":
            * - поле для ввода слова становится доступным только для чтения
            * - кнопка запуска становится неактивной
            * - кнопка применения меняется на кнопкук "галочки"
            * - текстовое поле становится неактивным
            */

            if (Mode == AppMode.ModeCode)
            {
                ResultTextBox.Text = "";

                ResultTextBox.IsReadOnly = true;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = false;
            }

            /* В режиме "слово":
            * - поле для ввода слова становится недоступным для редактирования
            * - кнопка запуска становится неактивной
            * - кнопка применения меняется на кнопкук "галочки"
            * - текстовое поле становится активным
            */

            if (Mode == AppMode.ModeGuess)
            {
                ResultTextBox.IsReadOnly = true;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = true;
            }
        }
    }
}
