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
		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int UserID { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(55)]
		[Index("UK_User_Email", IsUnique = true)]
		[UniqueEmail]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string Salt { get; set; }

		[Required]
		public string CompleteName { get; set; }

		[Required]
		[ForeignKey("Role")]
		public int RoleID { get; set; }

		#region Navigation properties

		public virtual Role Role { get; set; }

		public virtual ICollection<LoginHistory> LoginHistories { get; set; }

		public virtual ICollection<Reservation> Reservations { get; set; }

		#endregion

		#region Calculated properties

		public LoginHistory LastConnection
		{
			get
			{
				return LoginHistories.LastOrDefault();
			}
		}

		public Reservation LastReservation
		{
			get
			{
				return Reservations.LastOrDefault();
			}
		}

        public string CodeBare
        {
            get 
            {
                return Email.Substring(0, 4) + UserID;
            }
        }

		#endregion
	}
}
