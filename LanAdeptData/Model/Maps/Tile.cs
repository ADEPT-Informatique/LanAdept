using LanAdeptData.Model.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Maps
{
    public class Tile
    {
        public int TileID { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        #region Navigation properties
        public virtual Place Place { get; set; }
        #endregion
    }
}
