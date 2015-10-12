using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;

namespace LanAdeptAdmin.Views.Tournaments.ModelController
{
	public class TournamentModel
	{
		[DisplayName("Heure de début")]
		[Required(ErrorMessage="L'heure de début du tournoi est requise")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		public bool IsStarted { get; set; }

		public bool IsOver { get; set; }

		[DataType(DataType.MultilineText)]
		[DisplayName("Informations et règlements")]
		public string Info { get; set; }

		[DisplayName("Nb. de joueurs par équipe")]
		public int MaxPlayerPerTeam { get; set; }

		public int Id { get; set; }

		public int GameID { get; set; }

		public MvcHtmlString GetStatus()
		{
			string classeEtat = "";
			string messageEtat = "";

			if (!IsStarted && !IsOver)
			{
				classeEtat = "label-default";
				messageEtat = "Préparation";
			}
			else if (!IsOver)
			{
				classeEtat = "label-danger";
				messageEtat = "En cours";
			}
			else
			{
				classeEtat = "label-primary";
				messageEtat = "Terminé";
			}

			return new MvcHtmlString("<div class=\"label " + classeEtat + "\">" + messageEtat + "</div>");
		}


		#region Navigation properties
		[Display(Name = "Jeu")]
		public virtual LanAdeptData.Model.Game Game { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		#endregion

		#region Constructors

		public TournamentModel() {

		}

		public TournamentModel(Tournament tournament) {
			StartTime = tournament.StartTime;
			MaxPlayerPerTeam = tournament.MaxPlayerPerTeam;
			Game = tournament.Game;
			GameID = tournament.GameID;
			Teams = tournament.Teams;
			Id = tournament.TournamentID;
			IsOver = tournament.IsOver;
			IsStarted = tournament.IsStarted;
			Info = tournament.Info;
		}

		#endregion
	}
}