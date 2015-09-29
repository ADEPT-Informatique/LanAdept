using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Views.Place.ModelController
{
	public class SearchModel
	{
		[StringLength(int.MaxValue, MinimumLength=1)]
		public string Query { get; set; }

		public IEnumerable<User> UsersFound { get; set; }

		public bool FoundManyUser
		{
			get { return UsersFound != null && UsersFound.Count() > 1; }
		}
	}
}