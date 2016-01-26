using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
	public class GamerTagRepository : GenericRepository<GamerTag>
	{
		public GamerTagRepository(LanAdeptDataContext context) : base(context) { }

        public IEnumerable<GamerTag> GetByUser(User user)
        {
            return this.Get().Where(x => x.UserID == user.Id);
        }

        public GamerTag GetByUserAndGamerTagID(User user, int gamerTagID)
        {
            return this.Get().Where(x => x.UserID == user.Id && x.GamerTagID == gamerTagID).First();
        }

        public bool HasSameGamerTag(User user, string gamerTag)
        {
            return this.Get().Where(x => x.UserID == user.Id && x.Gamertag == gamerTag).Count() != 0;
        }
	}
}

