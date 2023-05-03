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

namespace House_Designer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Label> Rooms = new List<Label>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window Win = sender as Window;
            if (Win != null)
            {
                Point point = Mouse.GetPosition(PlaceCanvas);
                
                

                Label Room = new Label()
                {
                    Width = 100,
                    Height = 100,
                    Content = "New Room",
                    BorderThickness = new Thickness(5),
                    BorderBrush = Brushes.LightGray,
                    Foreground = Brushes.Red
                };
                PlaceCanvas.Children.Add(Room);
                if (!CheckCollision(point.X, point.Y, Room))
                {
                    Room.SetValue(LeftProperty, point.X);
                    Room.SetValue(TopProperty, point.Y);
                }
                
                Room.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                Rooms.Add(Room);

                InputWindow SettingWin = new InputWindow();
                SettingWin.Show();
            }
        }
        private bool CheckCollision(double X, double Y, Label NewRoom)
        {
            foreach (Label room in Rooms)
            {
                if (X >= (double)room.GetValue(LeftProperty) && X < (double)room.GetValue(LeftProperty) + room.Width)
                {
                    MessageBox.Show("COLLIDES");
                    NewRoom.SetValue(LeftProperty, (double)room.GetValue(LeftProperty) + room.Width);
                    NewRoom.SetValue(TopProperty, (double)room.GetValue(TopProperty));
                    return true;
                }
                MessageBox.Show($"{X}, {room.GetValue(LeftProperty)}");
            }


            return false;
        }
    }
}
