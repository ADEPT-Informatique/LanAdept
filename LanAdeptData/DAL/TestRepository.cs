using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL
{
	public class TestRepository : GenericRepository<Test>
	{
		public TestRepository(LanAdeptDataContext context)
			: base(context)
		{
		}
	}
}
