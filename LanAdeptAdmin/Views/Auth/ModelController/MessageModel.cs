using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Auth.ModelController
{
	public class MessageModel
	{
		public string Titre { get; set; }
		public string Contenu { get; set; }
		public AuthMessageType Type { get; set; }

		public string CssTitleClass
		{
			get
			{
				switch (Type)
				{
					case AuthMessageType.Success:
						return "text-success";
					case AuthMessageType.Error:
						return "text-danger";
				}

				return String.Empty;
			}
		}
	}

	public enum AuthMessageType
	{
		Success,
		Error
	}
}