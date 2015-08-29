using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
    public class Team
    {
        public int TeamID { get; set; }

        public string Name { get; set; }

        public virtual IEnumerable<User> Users { get; set; }
    }
}
