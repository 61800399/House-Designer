using System;
using System.Windows;
using System.Windows.Controls;
namespace House_Designer
{
    public class Floor : Canvas
    {
        public int FloorLevel { get; set; }
        public Floor()
        {
            FloorLevel = 0;
        }
        public Floor(int FL)
        {
            FloorLevel = FL;
        }
        public Floor(Thickness Margin)
        {
            FloorLevel = 0;
            this.Margin = Margin;
        }
        public Floor(int FL, Thickness Margin)
        {
            FloorLevel = FL;
            this.Margin = Margin;
        }
    }

}
