﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using LanAdeptData.Model.Users;

namespace LanAdeptData.DAL.Users
{
	public class GuestRepository : GenericRepository<Guest>
	{
		public GuestRepository(LanAdeptDataContext context) : base(context) { }

	}
}
