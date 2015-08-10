using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.Validation;

namespace LanAdeptData.Model
{
	public class User
	{
		public int UserID { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(55)]
		[Index("UK_User_Email", IsUnique = true)]
		[UniqueEmail]
		public string Email { get; set; }

		[StringLength(50, MinimumLength = 4)]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string Salt { get; set; }

		[Required]
		[ForeignKey("Role")]
		public int RoleID { get; set; }

		#region Navigation properties

		public virtual Role Role { get; set; }

		public virtual ICollection<LoginHistory> LoginHistories { get; set; }

		#endregion

		#region Calculated properties

		public LoginHistory LastConnection
		{
			get
			{
				return LoginHistories.OrderByDescending(x => x.Date).FirstOrDefault();
			}
		}

		#endregion
	}
}
