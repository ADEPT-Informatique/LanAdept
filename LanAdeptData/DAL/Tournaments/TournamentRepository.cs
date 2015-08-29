using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
    public class TournamentRepository : GenericRepository<Tournament>
    {
        public TournamentRepository(LanAdeptDataContext context) : base(context) { }
    }
}
