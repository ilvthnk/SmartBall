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
    }
}
