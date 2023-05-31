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
    /// Interaction logic for NewRoomWin.xaml
    /// </summary>
    public partial class NewRoomWin : Window
    {
        private MainWindow Main;
        public NewRoomWin(MainWindow M)
        {
            InitializeComponent();
            Main = M;
        }
        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
