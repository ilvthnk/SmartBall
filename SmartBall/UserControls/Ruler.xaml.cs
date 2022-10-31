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
        public int size = 10;
        public int ballPos = 0;
        public Label ball;
        public List<RowDefinition> rows = new List<RowDefinition>();
        public List<Grid> grids = new List<Grid>();

        public StringBuilder Text = new StringBuilder(String.Empty, 20);
        public Dictionary<int, object> textboxids = new Dictionary<int, object>();
        public void setBallPos(int bpos)
        {
            ballPos = bpos;
            Grid.SetRow(ball, bpos);
        }
        public Ruler()
        {
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
            InitializeComponent();
            
            //RulerArea.ShowGridLines = true;
            Grid.SetColumn(slider, 2);
            Grid.SetRow(slider, 1);

            Grid.SetColumn(ball, 0);
            setBallPos(size - 1);

            RulerArea.Children.Add(ball);
        }
        public void ClearField(int start, int end)
        {
            for (int i = 0; i < (end - start); i++)
            {
                RulerArea.Children.Remove(grids[i]);
                grids.Remove(grids[i]);
                RulerArea.RowDefinitions.Remove(rows[i]);
                rows.Remove(rows[i]);
            }
            for (int i = start; i < end; i++) //чистка (работает?)
                textboxids.Remove(i);
            for (int i = 0; i < grids.Count; i++)
                Grid.SetRow(grids[i], i);
            if(ballPos - (end - start) >= 0)
                setBallPos(ballPos - (end - start)); // почему то дают исключение
            else
                setBallPos(0);
        }
        public void UpdateField(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                RowDefinition rDef = new RowDefinition();
                rows.Add(rDef);
                RulerArea.RowDefinitions.Add(rDef);
            }
            List<Grid> newGrids = new List<Grid>();
            for (int i = end - 1, j = start; i >= start && j < end; i--, j++)
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
                letterInput.Text = String.Empty;
                letterInput.Width = 24;

                textboxids.Add(j, letterInput);

                Grid.SetColumn(letterInput, 2);

                part.Children.Add(letterInput);
                newGrids.Add(part);

                Grid.SetColumn(part, 1);

                RulerArea.Children.Add(part);
            }
            newGrids.AddRange(grids);
            grids = newGrids;
            for (int i = 0; i < grids.Count; i++)
                Grid.SetRow(grids[i], i);
            setBallPos(ballPos + (end - start));
        }
        public void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue)
                UpdateField((int)e.OldValue, (int)e.NewValue);
            else
                ClearField((int)e.NewValue, (int)e.OldValue);
            size = (int)e.NewValue;
            Grid.SetRowSpan(slider, size - 2);
        }
    }
}
