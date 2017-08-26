using LanAdeptData.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LanAdeptAdmin.Models
{
    public class TeamModel
	{
		public int TeamID { get; set; }

		[Required]
		[StringLength(30, MinimumLength = 4, ErrorMessage = "Le nom de l'équipe doit être entre 4 et 30 charactères")]
		[Display(Name = "Nom de l'équipe")]
		public string Name { get; set; }

		[StringLength(4, MinimumLength = 1, ErrorMessage = "Le tag doit comporter entre 1 et 4 charactères")]
		[Display(Name = "Tag")]
		public string Tag { get; set; }

		public bool IsComplete { get; set; }

		public bool IsReady { get; set; }

		public bool IsConfirmed { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

		public Tournament Tournament { get; set; }

        public ICollection<GamerTag> TeamGamerTags { get; set; }

		public TeamModel()
		{

		}

		public TeamModel(Team team)
		{
			TeamID = team.TeamID;
			Name = team.Name;
			Tag = team.Tag;
            TeamLeaderTag = team.TeamLeaderTag;
			TournamentID = team.TournamentID;
			Tournament = team.Tournament;
			TeamGamerTags = team.GamerTags;
			IsComplete = team.IsComplete;
			IsConfirmed = team.IsConfirmed;
			IsReady = team.IsReady;
		}
	}
}