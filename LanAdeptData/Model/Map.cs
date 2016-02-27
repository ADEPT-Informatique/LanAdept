using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
    public class Map
    {
        public int MapID { get; set; }
        public string MapName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        #region Navigation properties
        public virtual ICollection<Place> Places { get; set; }
        #endregion
    }
}
