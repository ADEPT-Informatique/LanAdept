using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdept.Models
{
    public class TeamDemandeModel
	{
		public List<Demande> Demandes { get; set; }

		public Team Team { get; set; }
	}
}