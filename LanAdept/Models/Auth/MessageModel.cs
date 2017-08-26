using System;

namespace LanAdept.Models
{
    public class MessageModel
	{
		public string Title { get; set; }
		public string Content { get; set; }
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