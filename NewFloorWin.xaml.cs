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
    /// Interaction logic for NewFloorWin.xaml
    /// </summary>
    public partial class NewFloorWin : Window
    {
        MainWindow Main;
        public NewFloorWin(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void RoomBut_Click(object sender, RoutedEventArgs e)
        {
            Button S = sender as Button;
            int index = int.Parse(S.Tag.ToString());
            Main.FloorAdd((FloorAddLevel)index); // The Tag Above = 0, Below = 1
            this.Close();
        }

    }
    public enum FloorAddLevel
    {
        None = -1,
        Above = 0,
        Bottom = 1,
        SpecifiedIndex = 2
    }
}
