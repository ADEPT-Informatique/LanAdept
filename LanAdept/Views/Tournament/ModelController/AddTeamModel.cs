using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournament.ModelController
{
    public class AddTeamModel
    {
        public int Id { get; set; }

        public int TournamentID { get; set; }

        public int UserID { get; set; }

        public User TeamLeader { get; set; }
    }
}