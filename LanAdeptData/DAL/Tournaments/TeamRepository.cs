using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
    public class TeamRepository : GenericRepository<Team>
    {
        public TeamRepository(LanAdeptDataContext context) : base(context) { }


        /// <summary>
        /// Find if the user owns currently a team
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>true if he has a team</returns>
        public bool IsTeamLeader(User user)
        {
            return Get().Where(x => x.TeamLeaderTag.UserID == user.UserID).FirstOrDefault() != null;
        }

        /// <summary>
        /// Find if the user owns currently a team in a tournament
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>true if he has a team</returns>
        public Team UserTeamInTournament(User user, Tournament tournament)
        {
            return Get().Where(x => x.GamerTags.Where(y => y.UserID == user.UserID).FirstOrDefault() != null 
                && x.TournamentID == tournament.TournamentID).FirstOrDefault();
        }
    }
}
