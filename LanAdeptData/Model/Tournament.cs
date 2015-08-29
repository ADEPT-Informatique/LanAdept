using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
    public class Tournament
    {
        public int TournamentID { get; set; }

        public virtual Game Game { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
