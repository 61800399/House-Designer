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
    /// Interaction logic for EditFloors.xaml
    /// </summary>
    public partial class EditFloors : Window
    {
        private MainWindow Main;
        private Floor SelectedFloor = null;
        private bool blockInput = false;
        public EditFloors(MainWindow M)
        {
            InitializeComponent();
            Main = M;
            Main.FixFloors();
            UpdateScreen();            
        }
        private void FloorSelected(object sender, MouseButtonEventArgs e)
        {
            Label Lab = sender as Label;
            Floor floor = Lab.Tag as Floor;
            if (Lab != null && floor != null)
            {
                blockInput = true;
                FloorSettings.Visibility = Visibility.Visible;
                FloorName.Text = Lab.Content.ToString();
                FloorLevel.Text = floor.FloorLevel.ToString();
                SelectedFloor = floor;
                UpdateScreen();
                InternalTimer();
            }
            
        }
        private async void InternalTimer()
        {
            blockInput = true;
            for (int i = 0; i < 1; i++)
            {
                System.Threading.Thread.Sleep(100);
            }
            blockInput = false;
            await Task.Run(() => { });
        }
        private void FloorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (!this.IsInitialized || SelectedFloor == null || blockInput)
            {
                return;
            }
            SelectedFloor.FloorName = FloorName.Text;
            UpdateScreen();
        }

        private void FloorLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            int.TryParse(box.Text, out int level);
            if (!this.IsInitialized || SelectedFloor == null || blockInput || level > Main.Floors.Count - 1)
            {
                return;
            }
            List<Floor> list = new List<Floor>();
            int count = 0;
            foreach (Floor floor in Main.Floors)
            {
                if (floor.FloorName == SelectedFloor.FloorName)
                {
                    floor.FloorLevel = level;
                    continue;
                }
                floor.FloorLevel = count;
                list.Add(floor);
            }
            list.Insert(level, SelectedFloor);
            Main.ChangeList(list);
            Main.FixSelector();
            FixList();
            UpdateScreen();
        }
        private void FixList()
        {
            for (int f = 0; f < Main.Floors.Count - 1; f++)
            {
                Main.Floors[f].FloorLevel = f;
            }
        }
        private void UpdateScreen()
        {
            FloorGrid.Children.Clear();
            int floorCount = 0;
            List<Floor> list = new List<Floor>();
            list.AddRange(Main.Floors);
            list.Reverse();
            foreach (Floor f in list)
            {
                Label label = new Label()
                {
                    Content = f.FloorName,
                    Background = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Tag = f,
                };
                label.MouseLeftButtonDown += FloorSelected;
                FloorGrid.Children.Add(label);
                floorCount++;
            }
            if (floorCount > 8)
            {
                this.Height += 200;
            }
        }
    }
}
