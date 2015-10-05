using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Views.General.ModelController
{
	public class RememberEmailModel
	{
		[DataType(DataType.MultilineText)]
		[DisplayName("Email de rappel")]
		public string RememberEmailContent { get; set; }
	}
}