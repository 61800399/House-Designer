using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Linq;

public class HouseRoom : Label
{
    public bool IsBaseRoom { get; set; } = false;
    public Dictionary<TilesSides, uint> SidesLeft { get; set; } = new Dictionary<TilesSides, uint>()
    {
        { TilesSides.Right, 1 },
        { TilesSides.Bottom, 1 },
        { TilesSides.Left, 1 },
        { TilesSides.Top, 1 },
    };
    
    public List<HouseRoom> AttachedRooms { get; set; } = new List<HouseRoom>();
    public HouseRoom()
    {
        this.Width = 100;
        this.Height = 100;
        this.Content = "New Room";
        this.BorderThickness = new Thickness(5);
        this.BorderBrush = Brushes.LightGray;
        this.Foreground = Brushes.Red;
    }
    public void UsedSide(TilesSides side)
    {
        foreach (TilesSides x in SidesLeft.Keys)
        {
            if (x == side)
            {
                SidesLeft[x] = 0;
                return;
            }
        }
        
    }
    public static bool IsClicked(Point Cursor, HouseRoom Room, double LeftProperty, double TopProperty)
    {
        return 
        (Cursor.X >= LeftProperty && 
        Cursor.X <= LeftProperty + Room.Width &&
        Cursor.Y >= TopProperty && 
        Cursor.Y <= TopProperty + Room.Height);
    }
    public enum TilesSides
    {
        None = -1,
        Right = 0,
        Bottom = 1,
        Left = 2,
        Top = 3,
        FreeForm = 4
    }
}

