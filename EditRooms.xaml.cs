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
    /// Interaction logic for EditRooms.xaml
    /// </summary>
    public partial class EditRooms : Window
    {
        private MainWindow Main;
        public EditRooms(MainWindow M)
        {
            InitializeComponent();
            Main = M;
            int floorCount = 0;
            foreach (Floor F in Main.Floors)
            {
                Label label = new Label()
                {
                    Content = F.FloorName,
                };
                
                FloorGrid.Children.Add(label);
                floorCount++;
            }
        }
    }
}
