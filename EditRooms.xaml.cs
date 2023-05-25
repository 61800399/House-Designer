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
                InternalTimer();
            }
            
        }
        private async void InternalTimer()
        {
            blockInput = true;
            for (int i = 0; i < 1; i++)
            {
                System.Threading.Thread.Sleep(500);
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
            if (!this.IsInitialized || SelectedFloor == null)
            {
                return;
            }
            TextBox box = sender as TextBox;
            List<Floor> list = new List<Floor>();
            if (box != null && int.TryParse(box.Text, out int final) && final < Main.Floors.Count)
            {
                for (int i = 0; i < Main.Floors.Count; i++)
                {
                    if (Main.Floors[i] == SelectedFloor)
                    {
                        continue;
                    }
                    list.Add(Main.Floors[i]);
                    Main.Floors[i].FloorLevel = i;
                }
                list.Insert(final, SelectedFloor);
                Main.ChangeList(list);
            }
            else
            {
                e.Handled = true;
            }
            UpdateScreen();
        }
        private void UpdateScreen()
        {
            FloorGrid.Children.Clear();
            int floorCount = 0;
            for (int f = Main.Floors.Count - 1; f >= 0; f--)
            {
                Label label = new Label()
                {
                    Content = Main.Floors[f].FloorName,
                    Background = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Tag = Main.Floors[f],
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
