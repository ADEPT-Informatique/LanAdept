using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
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
    }
}
