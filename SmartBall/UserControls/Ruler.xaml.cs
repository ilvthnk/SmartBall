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
using MaterialDesignThemes.Wpf;

namespace SmartBall.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Ruler.xaml
    /// </summary>
    public partial class Ruler : UserControl
    {
        public int Size = 10;

        public Ruler()
        {
            InitializeComponent();

            for (int i = 0; i < Size; i++)
            {
                RulerArea.RowDefinitions.Add(new RowDefinition());

                Grid part = new Grid();

                part.Margin = new Thickness(16, 0, 16, 0);

                part.ColumnDefinitions.Add(new ColumnDefinition());
                part.ColumnDefinitions.Add(new ColumnDefinition());
                part.ColumnDefinitions.Add(new ColumnDefinition());

                part.Margin = new Thickness(8, 0, 8, 0);

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
                letterInput.HorizontalAlignment = HorizontalAlignment.Right;
                letterInput.VerticalAlignment = VerticalAlignment.Center;
                letterInput.HorizontalContentAlignment = HorizontalAlignment.Center;
                letterInput.MaxLength = 1;
                letterInput.Text = "A";
                letterInput.Width = 24;

                Grid.SetColumn(letterInput, 2);

                part.Children.Add(letterInput);

                Grid.SetRow(part, i);

                RulerArea.Children.Add(part);
            }

            Label ball = new Label
            {
                Height = 24,
                Width = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Red
            };

            Style ballStyle = new Style
            {
                TargetType = typeof(Border),
                Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(24) } }
            };

            ball.Resources.Add(ballStyle.TargetType, ballStyle);

            Grid.SetColumn(ball, 2);

            Grid.SetRow(ball, 2);

            RulerArea.Children.Add(ball);
        }
    }
}
