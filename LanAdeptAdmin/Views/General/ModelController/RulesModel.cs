using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Views.General.ModelController
{
	public class RulesModel
	{
		[DataType(DataType.MultilineText)]
		[DisplayName("Règlements")]
		public string Rules { get; set; }
	}
}