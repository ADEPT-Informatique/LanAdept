using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Models
{
    public class UserSearchModel
	{
		[StringLength(int.MaxValue, MinimumLength = 1)]
		public string Query { get; set; }

		public int Page { get; set; }

		public IEnumerable<User> UsersFound { get; set; }

		public bool FoundUsers
		{
			get { return UsersFound != null && UsersFound.Count() > 0; }
		}

		public UserSearchModel()
		{
			UsersFound = new List<User>();
			Page = 1;
		}
	}
}