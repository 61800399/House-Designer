using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace House_Designer
{
    public class Floor : Canvas
    {
        public string FloorName { get; set; }
        public int FloorLevel { get; set; }
        public List<HouseRoom> Rooms { get; set; } = new List<HouseRoom>();
        public HouseRoom BaseRoom { get; set; } = null;
        public Dictionary<int ,HouseRoom> Stairwells { get; internal set; }
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
