using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
    public class GameRepository : GenericRepository<Game>
    {
        public GameRepository(LanAdeptDataContext context) : base(context) { }
    }
}
