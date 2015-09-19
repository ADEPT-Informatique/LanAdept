using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Settings
{
	public class SettingRepository : GenericRepository<Setting>
	{
		public SettingRepository(LanAdeptDataContext context) : base(context) { }

		public Setting GetCurrentSettings()
		{
			Setting setting = Get().FirstOrDefault();
			if(setting == null)
			{
				setting = new Setting();
				setting.StartDate = DateTime.Now;
				setting.EndDate = DateTime.Now.AddDays(1);

				Insert(setting);
				UnitOfWork.Current.Save();
			}

			return setting;
		}
	}
}
