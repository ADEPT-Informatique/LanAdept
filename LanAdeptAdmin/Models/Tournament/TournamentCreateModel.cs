using LanAdeptCore.Service.Challonge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Models
{
    public class TournamentCreateModel
    {
        [DisplayName("Heure de début")]
		[Required(ErrorMessage="L'heure de début du tournoi est requise")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		[DisplayName("Nb. de joueurs par équipe")]
		public int MaxPlayerPerTeam { get; set; }

		[Display(Name = "Jeu")]
		public string Game { get; set; }

        [Display(Name = "Utiliser Challonge")]
        public bool IsChallonge { get; set; }

        public TounamentType Type { get; set; }
    }
}