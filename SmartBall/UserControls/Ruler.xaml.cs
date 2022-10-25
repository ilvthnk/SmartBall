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

        public int ballPos = 0;
        public StringBuilder Text = new StringBuilder("JSFALSFASJ");
        Dictionary<object, int> textboxids = new Dictionary<object, int>();
        public void setBallPos(int bpos)
        {
            ballPos = bpos;
            Grid.SetRow(ball, bpos);
        }
        Label ball;
        public Ruler()
        {
            InitializeComponent();
            for (int i = 0; i < Text.Length; i++)            
                RulerArea.RowDefinitions.Add(new RowDefinition());
            //RulerArea.ShowGridLines = true;

            for (int i = 0; i < Text.Length; i++)
            {
                Grid part = new Grid();
                //part.ShowGridLines = true;
                part.ColumnDefinitions.Add(new ColumnDefinition());
                part.ColumnDefinitions.Add(new ColumnDefinition());
                part.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock number = new TextBlock();

                number.Text = (i + 1).ToString();
                number.HorizontalAlignment = HorizontalAlignment.Right;
                number.VerticalAlignment = VerticalAlignment.Center;
                number.FontSize = 16;

                Grid.SetColumn(number, 1);

                part.Children.Add(number);

                TextBox letterInput = new TextBox();

                MaterialDesignThemes.Wpf.TextFieldAssist.SetCharacterCounterVisibility(letterInput, Visibility.Hidden);
                letterInput.FontSize = 16;
                letterInput.HorizontalAlignment = HorizontalAlignment.Center;
                letterInput.VerticalAlignment = VerticalAlignment.Center;
                letterInput.HorizontalContentAlignment = HorizontalAlignment.Center;
                letterInput.MaxLength = 1;
                letterInput.Text = Text[i].ToString();
                letterInput.Width = 24;

                textboxids.Add(letterInput, i);

                Grid.SetColumn(letterInput, 2);

                part.Children.Add(letterInput);

                Grid.SetRow(part, i);
                Grid.SetColumn(part, 1);
                RulerArea.Children.Add(part);
            }
            Slider slider = new Slider
            {
                Orientation = Orientation.Vertical,
                Value = 70,
                Minimum = 1,
                Maximum = this.Text.Length,
                TickFrequency = 1,
                HorizontalAlignment = HorizontalAlignment.Center
                //Style="{StaticResource MaterialDesignDiscreteSlider}"
            };
            Grid.SetColumn(slider, 2);
            Grid.SetRow(slider, 5);
            Grid.SetRowSpan(slider, 5);

            ball = new Label
            {
                Height = 24,
                Width = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.Red
            };

            Style ballStyle = new Style
            {
                TargetType = typeof(Border),
                Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(24) } }
            };

            ball.Resources.Add(ballStyle.TargetType, ballStyle);

            Grid.SetColumn(ball, 0);

            setBallPos(ballPos);

            RulerArea.Children.Add(ball);
            RulerArea.Children.Add(slider);

            //RulerArea.ColumnDefinitions.Remove(Removal);
            //RulerArea.Children.Remove(slider);
        }
    }
}
