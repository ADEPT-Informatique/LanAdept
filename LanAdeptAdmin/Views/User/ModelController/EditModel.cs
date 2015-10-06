using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.Model;
using LanAdeptData.Validation;

namespace LanAdeptAdmin.Views.Users.ModelController
{
	public class EditModel
	{
		public int UserID { get; set; }

		[UniqueEmail]
		[Editable(false)]
		public string Email { get; set; }

		[Required]
		public string CompleteName { get; set; }

		[Required]
		public int RoleID { get; set; }

		public SelectList RoleList { get; set; }

		public EditModel() { }
		public EditModel(User user)
		{
			Email = user.Email;
			CompleteName = user.CompleteName;
			RoleID = user.RoleID;
		}

	}
}