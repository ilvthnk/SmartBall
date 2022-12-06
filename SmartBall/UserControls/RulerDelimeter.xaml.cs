using System.Windows;
using System.Windows.Controls;

namespace SmartBall.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RulerDelimeter.xaml
    /// </summary>
    public partial class RulerDelimeter : UserControl
    {
        public int Id;

        public RulerDelimeter(int id)
        {
            InitializeComponent();

            Id = id;

            Number.HorizontalContentAlignment = HorizontalAlignment.Right;
            Number.VerticalContentAlignment = VerticalAlignment.Center;
            Number.Content = id.ToString();

            MaterialDesignThemes.Wpf.TextFieldAssist.SetCharacterCounterVisibility(TBox, Visibility.Hidden);
        }

        private void CloseError(object sender, RoutedEventArgs e) 
        {
            
        }
    }
}
