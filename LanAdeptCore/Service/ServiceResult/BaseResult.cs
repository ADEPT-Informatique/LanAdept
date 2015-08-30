using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Service.ServiceResult
{
	public class BaseResult
	{
		public bool HasError { get; set; }

		public string Message { get; set; }
	}
}
