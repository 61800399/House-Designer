using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Linq;
using House_Designer;

public class HouseRoom : Label
{
    MainWindow Main;
    public bool IsGhost { get; private set; } = false;
    public bool IsBaseRoom { get; set; } = false;
    public Dictionary<TilesSides, uint> SidesLeft { get; set; } = new Dictionary<TilesSides, uint>()
    {
        { TilesSides.Right, 1 },
        { TilesSides.Bottom, 1 },
        { TilesSides.Left, 1 },
        { TilesSides.Top, 1 },
    };
    public RoomType Type { get; private set; }
    
    public List<HouseRoom> AttachedRooms { get; set; } = new List<HouseRoom>();
    public HouseRoom()
    {
        this.Width = 100;
        this.Height = 100;
        this.Content = "New Room";
        this.BorderThickness = new Thickness(5);
        this.BorderBrush = Brushes.LightGray;
        this.Foreground = Brushes.Red;
        this.Type = RoomType.Normal;
    }
    public HouseRoom(RoomType type, MainWindow M)
    {
        Main = M;
        this.BorderThickness = new Thickness(5);
        this.BorderBrush = Brushes.LightGray;
        this.Foreground = Brushes.Red;
        this.Type = type;
        Types();
    }
    private void Types()
    {
        if (this.Type == RoomType.Normal)
        {
            this.Width = 100;
            this.Height = 100;
            this.Content = "New Room";
        }
        else if (this.Type == RoomType.StairUp)
        {
            this.Width = 100;
            this.Height = 50;
            this.Content = "New StairsUp";
            
        }
        else if (this.Type == RoomType.StairDown)
        {
            this.Width = 100;
            this.Height = 50;
            this.Content = "New StairsDown";
        }
        else
        {
            throw new Exception("Not a valid room");
        }
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
    public Point GetLocation()
    {
        Point tl = new Point();
        tl.X = (double)this.GetValue(Window.LeftProperty);
        tl.Y = (double)this.GetValue(Window.TopProperty);
        return tl;
    }
    public void ToggleGhostMode()
    {
        ToggleGhostMode(false, true);
    }
    public void ToggleGhostMode(bool mode)
    {
        ToggleGhostMode(mode, false);
    }
    public void ToggleGhostMode(bool mode, bool toggled)
    {
        if (toggled)
        {
            this.IsGhost = !this.IsGhost;
        }
        else if (!toggled)
        {
            this.IsGhost = mode;
        }
        else
        {
            throw new Exception();
        }
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
    public enum RoomType
    {
        NA = -1,
        Normal = 0,
        StairUp = 1,
        StairDown = 2,
    }
}

