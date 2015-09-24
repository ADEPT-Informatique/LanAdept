using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Views.Tournaments.ModelController
{
	public class TournamentModel
	{
		[DataType(DataType.DateTime)]
		[Display(Name = "Heure")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		public int Id { get; set; }

		#region Navigation properties
		[Display(Name = "Jeu")]
		public virtual Game Game { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		#endregion

		public TournamentModel() {

		}

		public TournamentModel(Tournament tournament) {
			StartTime = tournament.StartTime;
			Game = tournament.Game;
			Teams = tournament.Teams;
			Id = tournament.TournamentID;
		}
	}
}