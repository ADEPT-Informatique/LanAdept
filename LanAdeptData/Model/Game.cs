using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
    public class Game
    {
        public int GameID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

		#region Navigation properties

		public virtual ICollection<Tournament> Tournaments { get; set; }

		#endregion
	}
}
