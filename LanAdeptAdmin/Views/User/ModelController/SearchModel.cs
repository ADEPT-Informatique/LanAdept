using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Views.Users.ModelController
{
	public class SearchModel
	{
		[StringLength(int.MaxValue, MinimumLength = 1)]
		public string Query { get; set; }

		public int Page { get; set; }

		public IEnumerable<User> UsersFound { get; set; }

		public bool FoundUsers
		{
			get { return UsersFound != null && UsersFound.Count() > 0; }
		}

		public SearchModel()
		{
			UsersFound = new List<User>();
			Page = 1;
		}
	}
}