using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SmartBall.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Ruler.xaml
    /// </summary>
    public partial class Ruler : UserControl
    {
        public int Size = 0;
        public int BallPos = 0;

        public StringBuilder Text = new StringBuilder(string.Empty, 20);

        public Dictionary<int, RulerDelimeter> RulerDelimeters = new Dictionary<int, RulerDelimeter>();

        public void SetBallPos(int bpos)
        {

            BallAnimation(BallPos, bpos);
            
            BallPos = bpos;
        }

        private void BallAnimation(int currPos, int nPos)
        {
            ColorAnimation ballDisappear = new ColorAnimation
            {
                From = Colors.Red,
                To = Colors.White,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                DecelerationRatio = 1.0
            };

            ColorAnimation ballAppear = new ColorAnimation
            {
                From = Colors.White,
                To = Colors.Red,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            RulerDelimeters[currPos].Ball.Background = new SolidColorBrush();
            RulerDelimeters[nPos].Ball.Background = new SolidColorBrush();
            RulerDelimeters[currPos].Ball.Background.BeginAnimation(SolidColorBrush.ColorProperty, ballDisappear);
            RulerDelimeters[nPos].Ball.Background.BeginAnimation(SolidColorBrush.ColorProperty, ballAppear);
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

            if (BallPos == Size) SetBallPos(BallPos - 1);

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
            else if (nVal < oVal && nVal >= 4)
            {
                for (int i = 0; i < oVal - nVal; i++)
                    RemoveLast();
            }
        }
    }
}
