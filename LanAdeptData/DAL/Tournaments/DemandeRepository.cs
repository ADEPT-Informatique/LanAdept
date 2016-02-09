using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using LanAdeptData.Model.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
	public class DemandeRepository : GenericRepository<Demande>
	{
		public DemandeRepository(LanAdeptDataContext context) : base(context) { }

		public List<Demande> GetByGamerTagId(int? gamerTagId)
		{
			List<Demande> demandes = new List<Demande>();

			foreach (Demande demande in Get())
			{
				if (demande.GamerTag.GamerTagID == gamerTagId)
				{
					demandes.Add(demande);
				}
			}

			return demandes;
		}

		public List<Demande> GetByTeamId(int? teamId)
		{
			List<Demande> demandes = new List<Demande>();

			foreach (Demande demande in Get())
			{
				if (demande.Team.TeamID == teamId)
				{
					demandes.Add(demande);
				}
			}

			return demandes;
		}
	}
}
