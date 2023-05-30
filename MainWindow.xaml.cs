using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace House_Designer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Floor> Floors { get; private set; } = new List<Floor>();
        protected InputWindow OpenWin = null;
        protected NewRoomWin RoomWin;
        public Floor CurrentFloor;
        private bool BlockInput = false;
        public HouseRoom.RoomType SelectedType { get; private set; } = HouseRoom.RoomType.Normal;
        public bool AttachMode { get; set; } = false;
        public bool PlaceMode { get; set; } = false;
        public HouseRoom AttachRoomSubject { get; set; }
        protected double ScrnWidth;
        protected double ScrnHeight;
        public MainWindow()
        {
            InitializeComponent();
            Floor BF = new Floor(0, PlaceCanvas.Margin);
            CurrentFloor = BF;
            //CurrentFloor.Margin = new Thickness(ScrnWidth, ScrnHeight, ScrnWidth, ScrnHeight);
            PlaceCanvas = null;
            Screen.Children.Add(CurrentFloor);
            Floors.Add(CurrentFloor);
            CurrentFloor.FloorName = "GroundFloor";
        }
        /// <summary>
        /// Player Clicks anywhere on the screen
        /// </summary>
        /// <param name="sender">Canvas</param>
        /// <param name="e">Mouse event</param>
        protected void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window Win = sender as Window;
            Point point = Mouse.GetPosition(CurrentFloor);
            
            if (PlaceModeBox.IsDropDownOpen || HouseLevel.IsDropDownOpen || point.X < 0 || point.Y < 0)
            {
                return;
            }
            ClickLabel.Visibility = Visibility.Collapsed;
            if (CurrentFloor.Rooms.Count > 0 && OpenWin != null)
            {
                OpenWin.Close();
            }
            if (PlaceMode)
            {
                foreach (HouseRoom i in CurrentFloor.Rooms)
                {
                    if (IsClicked(point, i))
                    {
                        AttachRoomSubject = i;
                        break;
                    }
                }
                PlaceRoom(point);
                RoomWin.Close();
            }
            else if (CurrentFloor.BaseRoom != null && PlaceModeBox.SelectedIndex != 4)
            {
                return;
            }
            if (AttachMode)
            {
                OpenWin.Close();
                RoomWin = new NewRoomWin(this);
                RoomWin.Show();
                TryAttach();
                return;
            }
            if (Win != null)
            {
                PlaceRoom(point);
            }
        }

        /// <summary>
        /// Places a room in at a designated point
        /// </summary>
        /// <param name="point"> the point coordinates fo the placement</param>
        protected void PlaceRoom(Point point)
        {
            #region Variables
            bool TempPlaceMode = false; // Creates bool for placement mode, defaults false
            HouseRoom.TilesSides side = HouseRoom.TilesSides.None; // sets side to default None
            HouseRoom Room = new HouseRoom(SelectedType, this); // creates room
            CurrentFloor.Stairwells.Add((int)SelectedType, Room);
            #endregion
            CurrentFloor.Rooms.Add(Room); // adds Room to list of Rooms
            CurrentFloor.Children.Add(Room); // puts Room on the canvas so its position properties can be edited
            #region PlaceMode
            if (PlaceMode) // Player previously pressed place square button
            {
                switch (PlaceModeBox.SelectedIndex) // determines which side the player wants to place the square on
                {
                    case 0:
                        side = HouseRoom.TilesSides.Right;
                        break;
                    case 1:
                        side = HouseRoom.TilesSides.Bottom;
                        break;
                    case 2:
                        side = HouseRoom.TilesSides.Left;
                        break;
                    case 3:
                        side = HouseRoom.TilesSides.Top;
                        break;
                    case 4:
                        side = HouseRoom.TilesSides.FreeForm;
                        break;
                    default:
                        side = HouseRoom.TilesSides.None;
                        return;
                }
                PlaceMode = false; // Property changed back
                TempPlaceMode = true; // Temperarally keeps placemode
            }
            if (TempPlaceMode && !TryAttach(side, Room) || point.X < 0 || point.Y < 0) // If room fails to attach - remove room
            {
                CurrentFloor.Children.Remove(Room);
                Room = null;
                PlaceMode = false;
                return;
            }
            if (CurrentFloor.BaseRoom != null)
            {
                CurrentFloor.BaseRoom.AttachedRooms.Add(Room);
            }
            #endregion
            #region Not PlaceMode
            if (CurrentFloor.Rooms.Count <= 1) // Creates the BaseRoom first placed room by default
            {
                Room.IsBaseRoom = true;
                CurrentFloor.BaseRoom = Room;
            }
            if (!TempPlaceMode || side == HouseRoom.TilesSides.FreeForm) // sets non Placemode Values
            {
                if (CheckCollision(point.X, point.Y)) // checks for collision with other squares
                {
                    CurrentFloor.Children.Remove(Room);
                    Room = null;
                    return;
                }
                Room.SetValue(LeftProperty, point.X);
                Room.SetValue(TopProperty, point.Y);
            }
            #endregion
            Room.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            Room.MouseRightButtonDown += SetProperties;
            MakePropertiesWin(Room);
        }

        /// <summary>
        /// Attempts to attach the room in HouseDesigner
        /// </summary>
        /// <returns>Whether the process succeded</returns>
        protected bool TryAttach()
        {
            return TryAttach(HouseRoom.TilesSides.None, null);
        }

        /// <summary>
        /// Attempts to attach the room in HouseDesigner with specific instruction
        /// </summary>
        /// <param name="Side">Side of square being attached to</param>
        /// <param name="Room">the room to be attached</param>
        /// <returns>Whether the process succeded</returns>
        protected bool TryAttach(HouseRoom.TilesSides Side, HouseRoom Room)
        {
            if (Side == HouseRoom.TilesSides.FreeForm)
            {
                return true;
            }
            foreach (HouseRoom i in CurrentFloor.Rooms)
            {
                HouseRoom.TilesSides AttachmentMode;
                double AttachCoord;
                double AttachRight = (double)AttachRoomSubject.GetValue(LeftProperty) + AttachRoomSubject.Width;
                double AttachBottom = (double)AttachRoomSubject.GetValue(LeftProperty);
                double AttachLeft = (double)AttachRoomSubject.GetValue(LeftProperty) - i.Width;
                double AttachTop = (double)AttachRoomSubject.GetValue(LeftProperty);
                int Index = OpenWin.AttachIndex;
                if (AttachRoomSubject.AttachedRooms.Contains(i))
                {
                    continue;
                }

                if (i == Room || i != AttachRoomSubject && IsClicked(Mouse.GetPosition(CurrentFloor), i))
                {
                    if (Side != HouseRoom.TilesSides.None)
                    {
                        Index = (int)Side;

                    }
                    switch (Index)
                    {
                        case 0:
                            AttachmentMode = HouseRoom.TilesSides.Right;
                            AttachCoord = AttachRight;
                            break;
                        case 1:
                            AttachmentMode = HouseRoom.TilesSides.Bottom;
                            AttachCoord = AttachBottom;
                            break;
                        case 2:
                            AttachmentMode = HouseRoom.TilesSides.Left;
                            AttachCoord = AttachLeft;
                            break;
                        case 3:
                            AttachmentMode = HouseRoom.TilesSides.Top;
                            AttachCoord = AttachTop;
                            break;
                        default:
                            AttachmentMode = HouseRoom.TilesSides.Right;
                            AttachCoord = AttachRight;
                            break;
                    }
                    if (AttachRoomSubject.SidesLeft[(HouseRoom.TilesSides)Index] == 0)
                    {
                        return false;
                    }
                    i.SetValue(LeftProperty, AttachCoord);
                    if (AttachmentMode == HouseRoom.TilesSides.Left || AttachmentMode == HouseRoom.TilesSides.Right)
                    {
                        i.SetValue(TopProperty, (double)AttachRoomSubject.GetValue(TopProperty));
                    }
                    else if (AttachmentMode == HouseRoom.TilesSides.Top)
                    {
                        i.SetValue(TopProperty, (double)AttachRoomSubject.GetValue(TopProperty) - i.Height);
                    }
                    else
                    {
                        i.SetValue(TopProperty, (double)AttachRoomSubject.GetValue(TopProperty) + AttachRoomSubject.Height);
                    }
                    AttachRoomSubject.UsedSide((HouseRoom.TilesSides)OpenWin.AttachIndex);
                    AttachMode = false;
                    AttachRoomSubject.AttachedRooms.Add(i);
                    break;
                }
            }
            return true;
        }
        /// <summary>
        /// Renames the HouseRoom
        /// </summary>
        /// <param name="Box">TextBox with the string to change the room name</param>
        /// <param name="room">the HouseRoom to have its content changed</param>
        internal protected void RenameRoom(TextBox Box, HouseRoom room)
        {
            room.Content = Box.Text;
        }

        /// <summary>
        /// Determines if the mouse is located over a room
        /// </summary>
        /// <param name="Cursor">The Coordinates of the mouse cursor</param>
        /// <param name="Room">The room in question</param>
        /// <returns>If the room is on the points given</returns>
        protected bool IsClicked(Point Cursor, HouseRoom Room)
        {
            return HouseRoom.IsClicked(Cursor, Room, (double)Room.GetValue(LeftProperty), (double)Room.GetValue(TopProperty));
        }

        /// <summary>
        /// When a HouseRoom is Right clicked, opens a new InputWindow to change rooms properties
        /// </summary>
        /// <param name="sender">HouseRoom clicked</param>
        /// <param name="e">Mouse Event</param>
        protected void SetProperties(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                e.Handled = true;
                MakePropertiesWin(sender as HouseRoom);
            }

        }

        /// <summary>
        /// Checks for collision between rooms, mostly unused
        /// </summary>
        /// <param name="X">X Coordinate</param>
        /// <param name="Y">Y coordinate</param>
        /// <returns>Whether a collision occured</returns>
        protected bool CheckCollision(double X, double Y)
        {
            foreach (HouseRoom room in CurrentFloor.Rooms)
            {
                if (X >= (double)room.GetValue(LeftProperty) && X < (double)room.GetValue(LeftProperty) + room.Width && Y >= (double)room.GetValue(TopProperty) && Y < (double)room.GetValue(TopProperty) + room.Height)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates InputWindow for a room
        /// </summary>
        /// <param name="Subject">Room to make window for</param>
        protected void MakePropertiesWin(HouseRoom Subject)
        {
            if (OpenWin != null)
            {
                OpenWin.Close();
            }
            InputWindow SettingWin = new InputWindow(this, Subject, CurrentFloor.Rooms, ScrnWidth, ScrnHeight);

            SettingWin.Show();
            OpenWin = SettingWin;
        }

        protected void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Window Win = sender as Window;
            if (Win != null)
            {
                ScrnWidth = Canvas.Width;
                ScrnHeight = Canvas.Height;
            }
        }

        /// <summary>
        /// The button to place a new room was clicked
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Routed Event</param>
        protected void PlaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlaceMode && RoomWin != null)
            {
                RoomWin.Close();
            }
            else
            {
                RoomWin = new NewRoomWin(this);
                RoomWin.Show();
                PlaceMode = true;
            }

        }


        private void AddFloor_Click(object sender, RoutedEventArgs e)
        {
            NewFloorWin newFloor = new NewFloorWin(this);
            newFloor.Show();
        }

        private void HouseLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsInitialized || BlockInput)
            {
                return;
            }
            CurrentFloor.Visibility = Visibility.Collapsed;
            CurrentFloor = Floors[HouseLevel.SelectedIndex];
            CurrentFloor.Visibility = Visibility.Visible;
        }
        public void FloorAdd(FloorAddLevel Level)
        {
            FloorAdd(Level, -1);
        }
        public void FloorAdd(FloorAddLevel Level, int Specified_index)
        {
            int floorLevel = -1;
            if (Level == FloorAddLevel.Above)
            {
                floorLevel = HouseLevel.SelectedIndex;
                if (CurrentFloor.Stairwells.Count > 0) // finish adding stairwells between floors
                {

                }
            }
            else if (Level == FloorAddLevel.Bottom)
            {
                floorLevel = HouseLevel.SelectedIndex + 1;
            }
            else if (Level == FloorAddLevel.SpecifiedIndex)
            {
                floorLevel = Specified_index;
            }
            else
            {
                return;
            }
            Floor floor = new Floor(floorLevel, CurrentFloor.Margin);
            Screen.Children.Add(floor);
            Floors.Insert(floorLevel, floor);
            AddComboBoxItem(HouseLevel, floor);
            NewCurrentFloor(floor);
            HouseLevel.SelectedIndex = floorLevel;
            floor.FloorName = HouseLevel.Text;
            for (int i = 0; i < Floors.Count; i++)
            {
                Floors[i].FloorLevel = i;
            }
        }
        private void NewCurrentFloor(Floor NewFloor)
        {
            CurrentFloor.Visibility = Visibility.Collapsed;
            CurrentFloor = NewFloor;
        }
        private void AddComboBoxItem(ComboBox box, Floor floor)
        {
            TextBox boxItem = new TextBox()
            {
                Text = "New Floor",
                IsReadOnly = true,
            };
            box.Items.Insert(floor.FloorLevel, boxItem);
        }
        
        private void LevelSelectClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            foreach (TextBox box in comboBox.Items)
            {
                box.IsReadOnly = true;
            }
        }

        private void EditFloorBut_Click(object sender, RoutedEventArgs e)
        {
            EditFloors edit = new EditFloors(this);
            edit.Show();
        }
        public List<Floor> ChangeList(List<Floor> list)
        {
            Floors = list;
            return Floors;
        }
        public void FixFloors()
        {
            if (Floors[0].FloorLevel == 0)
            {
                return;
            }
            List<Floor> list = Floors;
            list.Reverse();
            int count = 0;
            foreach (Floor floor in list)
            {
                floor.FloorLevel = count;
                count++;
            }
            FixSelector();
        }
        public void FixSelector()
        {
            BlockInput = true;
            HouseLevel.Items.Clear();
            foreach (Floor floor in Floors)
            {
                TextBox item = new TextBox()
                {
                    Text = floor.FloorName,
                    IsReadOnly = true,
                };
                HouseLevel.Items.Add(item);
            }
            HouseLevel.SelectedIndex = 0;
            BlockInput = false;
        }

        private void RoomSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            foreach (Label L in Modifiers.Children)
            {
                int.TryParse(label.Tag.ToString(), out int type);
                if (L == label)
                {
                    L.BorderBrush = Brushes.Blue;
                    SelectedType = (HouseRoom.RoomType)type;
                    continue;
                }
                L.BorderBrush = Brushes.LightGray;
            }
        }
    }
}
