using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Places
{
    public class MapRepository : GenericRepository<Map>
    {
        public MapRepository(LanAdeptDataContext context) : base(context) { }
    }
}
