using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptData.Validation
{
	public class UniqueUsernameAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			User userFromUsername = UnitOfWork.Current.UserRepository.GetUserByUsername(value.ToString());
			User currentUser = validationContext.ObjectInstance as User;

			if (userFromUsername != null && (currentUser == null || currentUser.UserID != userFromUsername.UserID))
			{
				return new ValidationResult("This " + validationContext.DisplayName + " is already taken");
			}

			return ValidationResult.Success;
		}
	}
}
