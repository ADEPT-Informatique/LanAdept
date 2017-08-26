using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Tournaments
{
    public class TournamentRepository : GenericRepository<Tournament>
    {
        public TournamentRepository(LanAdeptDataContext context) : base(context) { }
    }
}
