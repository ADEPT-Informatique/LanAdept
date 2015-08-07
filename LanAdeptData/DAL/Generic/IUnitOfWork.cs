using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Generic
{
	internal interface IUnitOfWork : IDisposable
	{
		void Save();
	}
}
