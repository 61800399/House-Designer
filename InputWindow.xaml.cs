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
using System.Windows.Shapes;

namespace House_Designer
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        private HouseRoom Subject;
        private MainWindow MainWin;
        private List<HouseRoom> Rooms;
        private double MainScrnWidth;
        private double MainScrnHeight;
        public int AttachIndex { get; set; }
        public InputWindow(MainWindow window, HouseRoom SubjectRoom, List<HouseRoom> F, double scrnwidth, double scrnheight)
        {
            InitializeComponent();
            MainWin = window;
            Subject = SubjectRoom;
            Rooms = F;
            MainScrnWidth = scrnwidth;
            MainScrnHeight = scrnheight;
            Subject = SubjectRoom;
            this.Title = Subject.Content.ToString();
            RoomName.Text = Subject.Content.ToString();
            WidthProp.Text = Subject.Width.ToString();
            heightProp.Text = Subject.Height.ToString();
            XCoordBox.Text = Subject.GetValue(LeftProperty).ToString();
            YCoordBox.Text = Subject.GetValue(TopProperty).ToString();
            if (SubjectRoom.IsBaseRoom)
            {
                BaseRoomCheck.IsChecked = true;
            }
        }
        public InputWindow GetWindow()
        {
            return this;
        }
        private void WidthProp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox box = sender as TextBox;
            e.Handled = !double.TryParse(e.Text, out double E);
            string BoxText = box.Text + E;
            double width = double.Parse(BoxText.ToString());
            if ((double)Subject.GetValue(LeftProperty) + width > MainScrnWidth)
            {
                e.Handled = true;
                return;
            }
            Subject.Width = width;
            ReAdjustRooms();
        }

        private void heightProp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox box = sender as TextBox;
            e.Handled = !double.TryParse(e.Text, out double E);
            string BoxText = box.Text + E;
            double Height = double.Parse(BoxText.ToString());
            if (Height > 600 || (double)Subject.GetValue(TopProperty) + Height > MainScrnHeight)
            {
                e.Handled = true;
                return;
            }
            Subject.Height = Height;
            ReAdjustRooms();
        }
        
        private void ReAdjustRooms()
        {
            ReAdjustRooms(0, HouseRoom.TilesSides.None);
        }
        private void ReAdjustRooms(double Distance, HouseRoom.TilesSides Direction)
        {
            if (Subject.IsBaseRoom || Direction == HouseRoom.TilesSides.None)
            {
                foreach (HouseRoom I in Rooms)
                {
                    Adjust(I, Direction, Distance);
                }
            }
            else
            {
                Adjust(Subject, Direction, Distance);
            }
            
        }
        private void Adjust(HouseRoom room, HouseRoom.TilesSides Direction, double Distance)
        {
            if (Subject.AttachedRooms.Contains(room))
            {
                switch (Direction)
                {
                    case HouseRoom.TilesSides.Left:
                        room.SetValue(LeftProperty, (double)room.GetValue(LeftProperty) - Distance);
                        break;
                    case HouseRoom.TilesSides.Right:
                        room.SetValue(LeftProperty, (double)room.GetValue(LeftProperty) + Distance);
                        break;
                    case HouseRoom.TilesSides.Top:
                        room.SetValue(TopProperty, (double)room.GetValue(TopProperty) - Distance);
                        break;
                    case HouseRoom.TilesSides.Bottom:
                        room.SetValue(TopProperty, (double)room.GetValue(TopProperty) + Distance);
                        break;
                    default:
                        break;
                }
            }
            if (room != Subject &&
                (double)room.GetValue(LeftProperty) < (double)Subject.GetValue(LeftProperty) + Subject.Width &&
                (double)room.GetValue(LeftProperty) > (double)Subject.GetValue(LeftProperty)
                &&
                (double)room.GetValue(TopProperty) >= (double)Subject.GetValue(TopProperty) &&
                (double)room.GetValue(TopProperty) < (double)Subject.GetValue(TopProperty) + Subject.Height)
            {
                if ((double)Subject.GetValue(LeftProperty) + Subject.Width + room.Width > MainScrnWidth || 
                    (double)Subject.GetValue(TopProperty) + Subject.Height > MainScrnHeight)
                {
                    RemoveRoom(room);
                    return;
                }
                room.SetValue(LeftProperty, (double)Subject.GetValue(LeftProperty) + Subject.Width);
            }
        }
        private void RemoveRoom(HouseRoom Room)
        {
            Room.Background = Brushes.Transparent;
            Room.Foreground = Brushes.Transparent;
            Room.BorderThickness = new Thickness(0);
            Room = null; 
            Rooms.Remove(Room);
        }

        private void RoomName_TextInput(object sender, TextChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }
            MainWin.RenameRoom(sender as TextBox, Subject);
            this.Title = RoomName.Text;
        }

        private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }
            switch (ColorBox.SelectedIndex)
            {
                
                case 1:
                    Subject.Background = Brushes.IndianRed;
                    break;
                case 2:
                    Subject.Background = Brushes.Blue;
                    break;
                case 3:
                    Subject.Background = Brushes.Black;
                    break;
                default:
                    Subject.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF5F5F5");
                    break;
            }
        }
        private void BaseRoomCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }
            Subject.IsBaseRoom = !Subject.IsBaseRoom;
            MainWin.CurrentFloor.BaseRoom = Subject;
        }
        
        private void XCoordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox box = sender as TextBox;
            double BoxCoord;
            if (box.Text.Length > 0)
            {
                BoxCoord = double.Parse(box.Text.ToString());
            }
            else
            {
                BoxCoord = 0;
            }
            e.Handled = !double.TryParse(e.Text, out double E);
            string BoxText = box.Text + E;
            double Coordinate = double.Parse(BoxText.ToString());
            HouseRoom.TilesSides Direction;
            double distance;
            if (Coordinate + Subject.Width > MainScrnWidth)
            {
                e.Handled = true;
                return;
            }
            
            if (BoxCoord < Coordinate)
            {
                Direction = HouseRoom.TilesSides.Left;
                distance = (double)Subject.GetValue(LeftProperty) - Coordinate;
            }
            else if (BoxCoord > Coordinate)
            {
                Direction = HouseRoom.TilesSides.Right;
                distance = (double)Subject.GetValue(LeftProperty) - Coordinate;
            }
            else
            {
                throw new Exception("Impossible coordinate");
            }
            Subject.SetValue(LeftProperty, Coordinate);
            ReAdjustRooms(distance, Direction);
        }

        private void YCoordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
            TextBox box = sender as TextBox;
            double BoxCoord;
            if (box.Text.Length > 0)
            {
                BoxCoord = double.Parse(box.Text.ToString());
            }
            else
            {
                BoxCoord = 0;
            }
            e.Handled = !double.TryParse(e.Text, out double E);
            string BoxText = box.Text + E;
            double Coordinate = double.Parse(BoxText.ToString());
            HouseRoom.TilesSides Direction;
            double distance;
            if (Coordinate + Subject.Height > MainScrnHeight)
            {
                e.Handled = true;
                return;
            }

            if (BoxCoord < Coordinate)
            {
                Direction = HouseRoom.TilesSides.Top;
                distance = (double)Subject.GetValue(TopProperty) - Coordinate;
            }
            else if (BoxCoord > Coordinate)
            {
                Direction = HouseRoom.TilesSides.Bottom;
                distance = (double)Subject.GetValue(TopProperty) - Coordinate;
            }
            else if (BoxCoord == Coordinate)
            {
                Direction = HouseRoom.TilesSides.None;
                distance = 0;
            }
            else
            {
                throw new Exception("Impossible coordinate");
            }
            Subject.SetValue(TopProperty, Coordinate);
            ReAdjustRooms(distance, Direction);
        }
    }
}
