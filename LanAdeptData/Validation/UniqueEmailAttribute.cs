using System.ComponentModel.DataAnnotations;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptData.Validation
{
    public class UniqueEmailAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			User userFromEmail = UnitOfWork.Current.UserRepository.GetUserByEmail(value.ToString());
			User currentUser = validationContext.ObjectInstance as User;

			if (userFromEmail != null && (currentUser == null || currentUser.Id != userFromEmail.Id))
			{
				return new ValidationResult("Cette adresse email est déjà prise");
			}

			return ValidationResult.Success;
		}
	}
}
