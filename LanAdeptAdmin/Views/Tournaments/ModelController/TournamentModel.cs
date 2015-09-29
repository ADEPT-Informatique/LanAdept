using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LanAdeptAdmin.Views.Tournaments.ModelController
{
	public class TournamentModel
	{
		[DataType(DataType.DateTime)]
		[Display(Name = "Heure")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		public bool IsStarted { get; set; }

		public bool IsOver { get; set; }

		[DataType(DataType.MultilineText)]
		public string Info { get; set; }

		public int Id { get; set; }

		public int GameID { get; set; }

		#region Navigation properties
		[Display(Name = "Jeu")]
		public virtual LanAdeptData.Model.Game Game { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		#endregion

		public TournamentModel() {

		}

		public TournamentModel(Tournament tournament) {
			StartTime = tournament.StartTime;
			Game = tournament.Game;
			GameID = tournament.GameID;
			Teams = tournament.Teams;
			Id = tournament.TournamentID;
			IsOver = tournament.IsOver;
			IsStarted = tournament.IsStarted;
			Info = tournament.Info;

		}
	}
}