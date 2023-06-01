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
        public bool PlaceMode { get; set; } = true;
        public HouseRoom AttachRoomSubject { get; set; }
        protected double ScrnWidth;
        protected double ScrnHeight;
        public MainWindow()
        {
            InitializeComponent();
            Floor BF = new Floor(0, PlaceCanvas.Margin);
            CurrentFloor = BF;
            PlaceCanvas = null;
            Screen.Children.Add(CurrentFloor);
            Floors.Add(CurrentFloor);
            CurrentFloor.FloorName = "GroundFloor";
            ScaleBox.Width = ScrnWidth;
            ScaleBox.Height = ScrnHeight;
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
            foreach (HouseRoom Stair in CurrentFloor.Stairwells)
            {
                Stair.ToggleGhostMode(false);
            }
            if (PlaceModeBox.IsDropDownOpen || HouseLevel.IsDropDownOpen || point.X < 0 || point.Y < 0)
            {
                return;
            }
            if (ClickLabel.Visibility == Visibility.Visible)
            {
                PlaceMode = false;
                PlaceRoom(point);
                PlaceMode = true;
                ClickLabel.Visibility = Visibility.Collapsed;
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
                return;
            }
            else if (CurrentFloor.BaseRoom != null && PlaceModeBox.SelectedIndex != 4)
            {
                return;
            }
            if (AttachMode)
            {
                TryAttach();
                return;
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
            if (Room.Type != HouseRoom.RoomType.Normal)
            {
                CurrentFloor.Stairwells.Add(Room);
            }
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
                TempPlaceMode = true; // Temperarally keeps placemode
            }
            if (TempPlaceMode && !TryAttach(side, Room) || point.X < 0 || point.Y < 0) // If room fails to attach - remove room
            {
                CurrentFloor.Children.Remove(Room);
                Room.Visibility = Visibility.Collapsed;
                Room = null;
                if (OpenWin != null)
                {
                    OpenWin.Close();
                }
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
            try
            {
                if (Side == HouseRoom.TilesSides.FreeForm)
                {
                    return true;
                }
                if (Room == null || AttachRoomSubject == null)
                {
                    throw new ExceptionRoomNotAttachable();
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
                            throw new ExceptionRoomNotAttachable();
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
                        AttachRoomSubject.UsedSide((HouseRoom.TilesSides)PlaceModeBox.SelectedIndex);
                        AttachMode = false;
                        AttachRoomSubject.AttachedRooms.Add(i);
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ExceptionRoomNotAttachable)
            {
                return false;
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
        /// <summary>
        /// Finds all StairCases on the current floor
        /// </summary>
        /// <returns>A list of each staircase and the point it is located at</returns>
        private List<(HouseRoom, Point)> GetStairwells()
        {
            return GetStairwells(HouseRoom.RoomType.NA);
        }
        private List<(HouseRoom, Point)> GetStairwells(HouseRoom.RoomType Direction)
        {
            List<(HouseRoom, Point)> Stairwells = new List<(HouseRoom, Point)>();
            if (CurrentFloor.Stairwells.Count > 0)
            {
                foreach (HouseRoom room in CurrentFloor.Stairwells)
                {
                    if (room.Type != Direction && Direction != HouseRoom.RoomType.NA)
                    {
                        continue;
                    }
                    Point tl = room.GetLocation();
                    Stairwells.Add((room, tl));
                }
            }
            return Stairwells;
        }
        /// <summary>
        /// Creates multiple staircases on the current room
        /// </summary>
        /// <param name="Stairs">The list of staircases to place</param>
        private void BuildStairwells(List<(HouseRoom, Point)> Stairs)
        {
            PlaceMode = false;
            if (Stairs.Count > 0)
            {
                HouseRoom.RoomType oldType = SelectedType;
                HouseRoom.RoomType type = HouseRoom.RoomType.NA;
                for (int i = 0; i < Stairs.Count; i++)
                {
                    switch (Stairs[i].Item1.Type)
                    {
                        case HouseRoom.RoomType.StairUp:
                            type = HouseRoom.RoomType.StairDown;
                            break;
                        case HouseRoom.RoomType.StairDown:
                            type = HouseRoom.RoomType.StairUp;
                            break;
                    }
                    SelectedType = type;
                    PlaceRoom(Stairs[i].Item2);
                    CurrentFloor.Stairwells[i].ToggleGhostMode(true);
                    CurrentFloor.Stairwells[i].Opacity = 0.5;
                    SelectedType = oldType;
                }
            }
            PlaceMode = true;
        }
        public void FloorAdd(FloorAddLevel Level)
        {
            FloorAdd(Level, -1);
        }
        public void FloorAdd(FloorAddLevel Level, int Specified_index)
        {
            BlockInput = true;
            int floorLevel = -1;
            HouseRoom.RoomType type = HouseRoom.RoomType.NA;
            if (Level == FloorAddLevel.Above)
            {
                type = HouseRoom.RoomType.StairUp;
                floorLevel = HouseLevel.SelectedIndex;
            }
            else if (Level == FloorAddLevel.Bottom)
            {
                type = HouseRoom.RoomType.StairDown;
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
            List<(HouseRoom, Point)> StairWells = GetStairwells(type);
            NewCurrentFloor(floor);
            HouseLevel.SelectedIndex = floorLevel;
            floor.FloorName = HouseLevel.Text;
            for (int i = 0; i < Floors.Count; i++)
            {
                Floors[i].FloorLevel = i;
            }
            BuildStairwells(StairWells);
            BlockInput = false;
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
        public void DeleteRoom(HouseRoom room)
        {
            if (room.IsBaseRoom)
            {
                CurrentFloor.BaseRoom = null;
            }
            if (room.Type != HouseRoom.RoomType.Normal)
            {
                CurrentFloor.Stairwells.Remove(room);
            }
            CurrentFloor.Rooms.Remove(room);
            room.Visibility = Visibility.Collapsed;
            room = null;
            if (OpenWin != null)
            {
                OpenWin.Close();
            }
        }

        private void Scale_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScaleBox.Stretch = Stretch.Uniform;
            if (e.Delta > 0)
            {
                ScaleBox.Height += e.Delta;
                ScaleBox.Width += e.Delta;
            }
            else
            {
                ScaleBox.Height -= e.Delta;
                ScaleBox.Width -= e.Delta;
            }
        }
    }
}
