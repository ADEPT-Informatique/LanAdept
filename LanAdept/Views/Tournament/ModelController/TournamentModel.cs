using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournament.ModelController
{
	public class TournamentModel
	{
		[DataType(DataType.DateTime)]
		[Display(Name = "Heure")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		public int TournamentID { get; set; }

		public int GameID { get; set; }

		[Display(Name = "Jeu")]
		public Game Game { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public IEnumerable<GamerTag> GamerTags { get; set; }

		public TournamentModel()
		{

		}

		public TournamentModel(LanAdeptData.Model.Tournament tournament)
		{
			StartTime = tournament.StartTime;
			Game = tournament.Game;
			GameID = tournament.GameID;
			Teams = tournament.Teams;
			TournamentID = tournament.TournamentID;
		}
	}
}