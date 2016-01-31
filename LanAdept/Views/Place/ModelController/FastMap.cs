using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Place.ModelController
{
    public class FastMap
    {
        public int MapID { get; set; }
        public string MapName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        #region Navigation properties
        public LanAdeptData.Model.Place[,] TabPlaces { get; set; }
        #endregion

        public FastMap(Map map)
        {
            MapID = map.MapID;
            MapName = map.MapName;
            Width = map.Width;
            Height = map.Height;

            TabPlaces = new LanAdeptData.Model.Place[Width, Height];
            foreach (Tile tile in map.Tiles)
            {
                TabPlaces[tile.PositionX, tile.PositionY] = tile.Place;
            }
        }
    }
}