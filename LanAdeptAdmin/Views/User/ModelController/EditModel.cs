using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.Model;
using LanAdeptData.Validation;
using Microsoft.AspNet.Identity.EntityFramework;
using LanAdeptCore.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptData.DAL;
using LanAdeptData.Model.Users;

namespace LanAdeptAdmin.Views.Users.ModelController
{
	public class EditModel
	{
		public string UserID { get; set; }

		[Editable(false)]
		public string Email { get; set; }

		[Required]
		[DisplayName("Nom complet")]
		public string CompleteName { get; set; }

		public List<RoleModel> Roles { get; set; }

		public EditModel() { }
		public EditModel(User user, List<Role> roles)
		{
			UserID = user.Id;
			Email = user.Email;
			CompleteName = user.CompleteName;
			Roles = new List<RoleModel>();

			var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

			foreach (Role role in roles)
			{
				RoleModel roleModel = new RoleModel(role);

				if(userManager.IsInRole(user.Id, role.Name))
				{
					roleModel.UserInRole = true;
				}
				Roles.Add(roleModel);
			}

		}
	}
}