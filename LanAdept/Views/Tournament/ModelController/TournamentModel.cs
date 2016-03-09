using LanAdeptData.Model;
using LanAdeptData.Model.Tournaments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournaments.ModelController
{
	public class TournamentModel
	{
		[DataType(DataType.DateTime)]
		[Display(Name = "Heure de début")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		public int TournamentID { get; set; }

		[Display(Name = "Jeu")]
		public string Game { get; set; }

		[Display(Name = "Équipes")]
		public IEnumerable<TeamModel> Teams { get; set; }

		[Display(Name = "Membres")]
		public IEnumerable<GamerTag> GamerTags { get; set; }

        public bool IsConnected { get; set; }

		public bool IsStarted { get; set; }

		public bool IsOver { get; set; }

		public bool IsTeamLeader { get; set; }

		public string Info { get; set; }

		public int MaxPlayerPerTeam { get; set; }

		public bool CanAddTeam { get; set; }

        public string ChallongeURL { get; set; }

		public LanAdeptData.Model.Tournaments.Team UserTeam { get; set; }

		public TournamentModel(Tournament tournament)
		{
			StartTime = tournament.StartTime;
			Game = tournament.Game;
			TournamentID = tournament.TournamentID;
			Info = tournament.Info;
			IsStarted = tournament.IsStarted;
			IsOver = tournament.IsOver;
			MaxPlayerPerTeam = tournament.MaxPlayerPerTeam;
            ChallongeURL = tournament.ChallongeUrl;
		}
	}
}