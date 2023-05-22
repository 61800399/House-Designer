using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HouseDesigner
{
    public partial class ShapePropertiesWindow : Window
    {
        private readonly Shape shape;

        public ShapePropertiesWindow(Shape shape)
        {
            InitializeComponent();
            this.shape = shape;

            // Set the initial values of the textboxes and comboboxes
            txtWidth.Text = shape.Width.ToString();
            txtHeight.Text = shape.Height.ToString();
            txtStrokeThickness.Text = shape.StrokeThickness.ToString();
            cmbStrokeColor.SelectedItem = GetComboBoxItemByColor(shape.Stroke);
            cmbFillColor.SelectedItem = GetComboBoxItemByColor(shape.Fill);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            double width, height, strokeThickness;
            Color strokeColor, fillColor;

            // Parse the values from the textboxes and comboboxes
            if (!double.TryParse(txtWidth.Text, out width))
            {
                MessageBox.Show("Invalid width value.");
                return;
            }

            if (!double.TryParse(txtHeight.Text, out height))
            {
                MessageBox.Show("Invalid height value.");
                return;
            }

            if (!double.TryParse(txtStrokeThickness.Text, out strokeThickness))
            {
                MessageBox.Show("Invalid stroke thickness value.");
                return;
            }

            strokeColor = GetColorByComboBoxItem((ComboBoxItem)cmbStrokeColor.SelectedItem);
            fillColor = GetColorByComboBoxItem((ComboBoxItem)cmbFillColor.SelectedItem);

            // Update the shape properties
            shape.Width = width;
            shape.Height = height;
            shape.StrokeThickness = strokeThickness;
            shape.Stroke = new SolidColorBrush(strokeColor);
            shape.Fill = new SolidColorBrush(fillColor);

            // Close the window
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without updating the shape properties
            DialogResult = false;
        }

        private static Color GetColorByComboBoxItem(ComboBoxItem item)
        {
            switch (item.Content.ToString().ToLower())
            {
                case "black":
                    return Colors.Black;
                case "red":
                    return Colors.Red;
                case "green":
                    return Colors.Green;
                case "blue":
                    return Colors.Blue;
                case "white":
                    return Colors.White;
                case "gray":
                    return Colors.Gray;
                case "yellow":
                    return Colors.Yellow;
                default:
                    return Colors.Transparent;
            }
        }

        private static ComboBoxItem GetComboBoxItemByColor(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                switch (solidColorBrush.Color)
                {
                    case Colors.Black:
                        return new ComboBoxItem() { Content = "Black", IsSelected = true };
                    case Colors.Red:
                        return new ComboBoxItem() { Content = "Red", IsSelected = true };
                    case Colors.Green:
                        return new ComboBoxItem() { Content = "Green", IsSelected = true };
                    case Colors.Blue:
                        return new ComboBoxItem() { Content = "Blue", IsSelected = true };
                    case Colors.White:
                        return new ComboBoxItem() { Content = "White", IsSelected = true };
                    case Colors.Gray:
                        return new ComboBoxItem() { Content = "Gray", IsSelected = true };
                    case Colors.Yellow:
                        return new ComboBoxItem() { Content = "Yellow", IsSelected = true };
                    default:
                        return new ComboBoxItem() { Content = "Transparent", IsSelected = true };
                }
            }

            return new ComboBoxItem() { Content = "Transparent", IsSelected = true };
        }
    }
}