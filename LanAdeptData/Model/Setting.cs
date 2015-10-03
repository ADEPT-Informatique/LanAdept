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

		public string Rules { get; set; }

        public string Description { get; set; }

		public bool SendRememberEmail { get; set; }

		public int NbDaysBeforeRemember { get; set; }

		public string RememberEmailContent { get; set; }


		#region Calculated Properties

		public bool IsLanStarted { get { return DateTime.Now > StartDate; } }

		public bool IsLanOver { get { return DateTime.Now > EndDate; } }

		#endregion
	}
}
