using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SmartBall.UserControls;
using MaterialDesignThemes.Wpf;
using System.Text.Json;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Windows.Input;
using Microsoft.Win32;

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

        private bool IsEditing = true;

        private string Answer = String.Empty; // Для режима "слово"

        private AppMode Mode = AppMode.ModeCode; // По умолчанию

        private static RoutedCommand help = new RoutedCommand();

        private static RoutedCommand start = new RoutedCommand();

        private static RoutedCommand openF = new RoutedCommand();

        private static RoutedCommand saveF = new RoutedCommand();

        private static RoutedCommand ballUp = new RoutedCommand();

        private static RoutedCommand ballDown = new RoutedCommand();

        public MainWindow()
        {
            help.InputGestures.Add(new KeyGesture(Key.F1));
            start.InputGestures.Add(new KeyGesture(Key.F5));
            openF.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            saveF.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            ballUp.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));
            ballDown.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));

            CommandBindings.Add(new CommandBinding(help, Help));
            CommandBindings.Add(new CommandBinding(start, PlayButtonClicked));
            CommandBindings.Add(new CommandBinding(openF, FileImportButtonClicked));
            CommandBindings.Add(new CommandBinding(saveF, FileSaveButtonClicked));
            CommandBindings.Add(new CommandBinding(ballUp, onBallUp));
            CommandBindings.Add(new CommandBinding(ballDown, onBallDown));

            InitializeComponent();

            CheckCodeBtn.IsChecked = true;
        }

        private void closePop(object sender, EventArgs e)
        {
            popHelp.IsOpen = false;
        }

        private void onBallUp(object sender, RoutedEventArgs args)
        {
            if (!IsEditing)
                return;
            if (Ruler.BallPos < Ruler.Size - 1)
            {
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = false;
                Ruler.SetBallPos(Ruler.BallPos + 1);
            }
            else
            {
                Ruler.RulerDelimeters[Ruler.BallPos].error.Text = "Не могу :(";
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = true;
            }
        }

        private void onBallDown(object sender, RoutedEventArgs args)
        {
            if (!IsEditing)
                return;
            if (Ruler.BallPos > 0)
            {
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = false;
                Ruler.SetBallPos(Ruler.BallPos - 1);
            }
            else
            {
                Ruler.RulerDelimeters[Ruler.BallPos].error.Text = "Не могу :(";
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = true;
            }
        }

        // Для загрузки файла
        private void FileImportButtonClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (!IsEditing) Cancel();

                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

                dialog.InitialDirectory = "C:\\Users\\" + System.Environment.UserName + "\\AppData\\Local\\Programs\\SmartBall\\TaskLib";

                bool? result = dialog.ShowDialog();

                DemoAppTask task = new DemoAppTask();

                if (result == true)
                {
                    string fileName = dialog.FileName;

                    task = JsonSerializer.Deserialize<DemoAppTask>(File.ReadAllText(fileName));

                    if (task == null || task.Data == null)
                        throw new Exception("Ошибка чтения файла!");

                    task.Data = task.Data.Trim().Replace("\r\n", string.Empty).Replace(" ", string.Empty);

                    if (task.Data.Length < 4 || task.Data.Length > 20)
                        throw new Exception("На линейке должно быть от 4 и до 20 символов!");

                    Ruler.Slider.Value = task.Data.Length;

                    if (task.Position < 0 || task.Position >= task.Data.Length)
                        task.Position = 0;

                    if (Ruler.BallPos >= task.Data.Length)
                        Ruler.BallPos = task.Data.Length - 1;

                    Ruler.SetBallPos(task.Position);

                    Ruler.Text.Clear();

                    for (int i = 0; i < task.Data.Length; i++)
                    {
                        Ruler.RulerDelimeters[i].TBox.Text = task.Data[i].ToString();

                        Ruler.Text.Append(task.Data[i]);
                    }

                    ResultTextBox.Foreground = Brushes.Black;

                    Mode = AppMode.ModeGuess;

                    HintAssist.SetHint(WordTextBox, "Ваш ответ");

                    CodeTextBox.Text = task.Code;

                    CheckGuessBtn.IsChecked = true;

                    Apply();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "Ошибка!");
            }
        }

        //сохранение в файл
        private void FileSaveButtonClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (ApplyBtn.Kind == PackIconKind.CancelOutline) // только если изменения применены
                {
                    if (Mode != AppMode.ModeGuess)
                    {
                        Cancel();

                        return;
                    }

                    Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

                    dialog.Filter = "Text file (*.json)|*.json";

                    dialog.InitialDirectory = "C:\\Users\\" + System.Environment.UserName + "\\AppData\\Local\\Programs\\SmartBall\\TaskLib";

                    if (dialog.ShowDialog() == true)
                    {
                        DemoAppTask task = new DemoAppTask()
                        {
                            Position = Ruler.BallPos,
                            Code = CodeTextBox.Text,
                            Data = Ruler.Text.ToString() // в Ruler.Text будет актуальный текст только после сохранения изменений
                        };

                        string serialized = JsonSerializer.Serialize<DemoAppTask>(task);

                        File.WriteAllText(dialog.FileName, serialized);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "Ошибка!");
            }
        }

        // Запуск в данном режиме
        private void PlayButtonClicked(object sender, RoutedEventArgs args)
        {
            if (IsEditing)
                return;
            if (Mode == AppMode.ModeGuess && WordTextBox.Text == String.Empty)
            {
                MessageBox.Show(this, "Напиши слово!", "Ошибка");
                return;
            }
            if (Mode == AppMode.ModeGuess) 
            {
                foreach (var letter in WordTextBox.Text)
                    if (!Ruler.Text.ToString().Contains(letter))
                    {
                        MessageBox.Show(this, "Слово должно состоять из элементов линейки!", "Ошибка");
                        return;
                    }
            }

            CommandExecutor cmdex = new CommandExecutor(Ruler.RulerDelimeters, CodeTextBox.Text.Trim().Replace("\r\n", string.Empty), Ruler.BallPos);

            try
            {

                int SavedBallPos = Ruler.BallPos;

                if (CodeTextBox.Text == String.Empty) throw new Exception("Введи алгоритм!");

                while (!cmdex.IsStopped)
                {
                    cmdex.Next();

                    Ruler.SetBallPos(cmdex.DataCursor);
                }

                Answer = cmdex.Result;

                if (Mode == AppMode.ModeCode)
                {
                    if (WordTextBox.Text == Answer)
                        ResultTextBox.Foreground = Brushes.LawnGreen;
                    else
                        ResultTextBox.Foreground = Brushes.DarkRed;
                    ResultTextBox.Text = cmdex.Result;
                }

                if (Mode == AppMode.ModeGuess)
                {
                    if (WordTextBox.Text == Answer)
                    {
                        ResultTextBox.Foreground = Brushes.LawnGreen;
                        ResultTextBox.Text = Answer;
                    }
                    else
                    {
                        ResultTextBox.Foreground = Brushes.DarkRed;
                        ResultTextBox.Text = "Неверно";
                        Ruler.SetBallPos(SavedBallPos);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Ruler.RulerDelimeters[Ruler.BallPos].error.Text = "Не понимаю 0_o";
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = true;
            }
            catch (IndexOutOfRangeException)
            {
                Ruler.RulerDelimeters[Ruler.BallPos].error.Text = "Не могу :(";
                Ruler.RulerDelimeters[Ruler.BallPos].errorPop.IsOpen = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message, "Ошибка");
            }
        }

        // Переключение в режим "код"
        private void RBtnCodeChecked(object sender, RoutedEventArgs args)
        {
            WordTextBox.IsReadOnly = false;

            ResultTextBox.Foreground = Brushes.Black;

            if (Mode != AppMode.ModeCode)
            {
                HintAssist.SetHint(WordTextBox, "Слово, которое вы хотите составить");

                Mode = AppMode.ModeCode;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = false;

                WordTextBox.IsEnabled = true;
            }
        }

        // Переключение в режим "слово"
        private void RBtnGuessChecked(object sender, RoutedEventArgs args)
        {
            WordTextBox.Text = String.Empty;

            WordTextBox.IsReadOnly = true;

            ResultTextBox.Foreground = Brushes.Black;

            if (Mode != AppMode.ModeGuess)
            {
                HintAssist.SetHint(WordTextBox, "Ваш ответ");

                Mode = AppMode.ModeGuess;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = true;

                WordTextBox.IsEnabled = false;
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

            Ruler.Text.Clear();


            foreach (var pair in Ruler.RulerDelimeters)
            {
                if ((pair.Value).TBox.Text == string.Empty || (pair.Value).TBox.Text == " ")
                {
                    MessageBox.Show(this, "Заполни линейку (пробелы не считаются)!", "Ошибка");

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
                ResultTextBox.Text = String.Empty;

                PlayBtn.IsEnabled = true;

                ApplyBtn.Kind = PackIconKind.CancelOutline;

                CodeTextBox.IsEnabled = true;

                foreach (var letter in WordTextBox.Text)
                {
                    if (!Ruler.Text.ToString().Contains(letter)) 
                    {
                        Cancel();
                        MessageBox.Show(this, "Слово должно состоять из элементов линейки!", "Ошибка");
                        return;
                    }
                }

                if (WordTextBox.Text == String.Empty)
                {
                    Cancel();
                    MessageBox.Show(this, "Напиши слово!", "Ошибка");
                    return;
                }

                HintAssist.SetHint(WordTextBox, "Слово, которое вы хотите составить");
            }

            /* В режиме "слово":
            * - поле для ввода слова становится доступным для редактирования
            * - кнопка запуска становится активной
            * - кнопка применения меняется на кнопкук отмены
            * - текстовое поле становится неактивным
            */

            if (Mode == AppMode.ModeGuess)
            {
                WordTextBox.IsEnabled = true;

                WordTextBox.IsReadOnly = false;

                PlayBtn.IsEnabled = true;

                ApplyBtn.Kind = PackIconKind.CancelOutline;

                CodeTextBox.IsEnabled = true;

                CodeTextBox.IsReadOnly = true;

                if (CodeTextBox.Text == String.Empty)
                {
                    Cancel();
                    MessageBox.Show(this, "Напиши алгоритм!", "Ошибка");
                    return;
                }

                HintAssist.SetHint(WordTextBox, "Ваш ответ");
            }
            CheckBtn.ToolTip = "Изменить задание";
            IsEditing = false;
        }

        // Откат в режим редактирования (ничего не сохраняется)
        private void Cancel()
        {

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
                ResultTextBox.Text = String.Empty;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = false;

                WordTextBox.IsReadOnly = false;
            }


            /* В режиме "слово":
            * - поле для ввода слова становится недоступным для редактирования
            * - кнопка запуска становится неактивной
            * - кнопка применения меняется на кнопкук "галочки"
            * - текстовое поле становится активным
            */

            if (Mode == AppMode.ModeGuess)
            {
                WordTextBox.Text = String.Empty;

                WordTextBox.IsReadOnly = true;

                WordTextBox.IsEnabled= false;

                ResultTextBox.Text = String.Empty;

                PlayBtn.IsEnabled = false;

                ApplyBtn.Kind = PackIconKind.Check;

                CodeTextBox.IsEnabled = true;

                CodeTextBox.IsReadOnly = false;
            }
            CheckBtn.ToolTip = "Начать задание";
            IsEditing = true;
        }
        private void Help(object sender, RoutedEventArgs e) 
        {
            popHelp.IsOpen = !popHelp.IsOpen;
        }
    }
}
