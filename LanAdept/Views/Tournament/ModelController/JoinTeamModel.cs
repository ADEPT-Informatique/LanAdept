using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournaments.ModelController
{
    public class JoinTeamModel
    {
        public int? TournamentID { get; set; }

        public int? GamerTagID { get; set; }

        public int? TeamID { get; set; }

    }
}