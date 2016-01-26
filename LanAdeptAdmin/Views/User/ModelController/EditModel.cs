using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		public string UserID { get; set; }

		[UniqueEmail]
		[Editable(false)]
		public string Email { get; set; }

		[Required]
		[DisplayName("Nom complet")]
		public string CompleteName { get; set; }

		public SelectList RoleList { get; set; }

		public EditModel() { }
		public EditModel(User user)
		{
			UserID = user.Id;
			Email = user.Email;
			CompleteName = user.CompleteName;
		}

	}
}