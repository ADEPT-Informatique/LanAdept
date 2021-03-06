﻿using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
{
    public class ResetPasswordModel
	{
		public string Id { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Le nouveau mot de passe doit compter au moins {2} caractères.", MinimumLength = 4)]
		[DataType(DataType.Password)]
		[Display(Name = "Nouveau mot de passe")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirmer le nouveau mot de passe")]
		[Compare("Password", ErrorMessage = "Le mot de passe de confirmation est différent du nouveau mot de passe.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }

	}
}