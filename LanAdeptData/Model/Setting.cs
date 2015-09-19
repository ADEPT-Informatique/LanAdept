using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Setting
	{
		public int SettingId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public String Rules { get; set; }

		public bool SendRememberEmail { get; set; }

		public int NbDaysBeforeRemember { get; set; }

		public string RememberEmailContent { get; set; }


		#region Calculated Properties

		public bool IsLanStarted { get { return DateTime.Now > StartDate; } }

		#endregion
	}
}
