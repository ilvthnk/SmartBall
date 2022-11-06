using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace SmartBall.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Ruler.xaml
    /// </summary>
    public partial class Ruler : UserControl
    {
        public int Size = 0;
        public int BallPos = 0;

        public StringBuilder Text = new StringBuilder(String.Empty, 20);

        public Dictionary<int, RulerDelimeter> RulerDelimeters = new Dictionary<int, RulerDelimeter>();

        public void SetBallPos(int bpos)
        {
            RulerDelimeters[BallPos].Ball.Background = Brushes.White;

            BallPos = bpos;

            RulerDelimeters[BallPos].Ball.Background = Brushes.Red;
        }

        public void AppendDelimeter()
        {
            Size++;

            RulerDelimeter rd = new RulerDelimeter(Size);

            RulerDelimeters.Add(Size - 1, rd);

            DockPanel.SetDock(rd, Dock.Bottom);

            TextBoxArea.Children.Add(rd);
        }

        public void RemoveLast()
        {
            Size--;

            var rd = RulerDelimeters[Size];

            TextBoxArea.Children.Remove(rd);

            RulerDelimeters.Remove(Size);
        }

        public Ruler()
        {
            InitializeComponent();

            SetBallPos(0);
        }

        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> props)
        {
            int nVal = (int)props.NewValue;
            int oVal = (int)props.OldValue;

            if (nVal > oVal && nVal <= 20)
            {
                for (int i = 0; i < nVal - oVal; i++)
                    AppendDelimeter();
            }
            else if (nVal < oVal && nVal >= 10)
            {
                for (int i = 0; i < oVal - nVal; i++)
                    RemoveLast();
            }
        }
    }
}
