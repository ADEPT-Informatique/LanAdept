using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.Validation;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace LanAdeptData.Model
{
	public class User : IdentityUser
	{
		[Required]
		public string CompleteName { get; set; }

		#region Navigation properties

		public virtual ICollection<Reservation> Reservations { get; set; }

		public virtual ICollection<Team> Teams { get; set; }

		#endregion

		#region Calculated properties

		public Reservation LastReservation
		{
			get
			{
				return Reservations.LastOrDefault();
			}
		}

        public string Barcode
        {
            get 
            {
                UInt32 hashCode = (UInt32)Email.GetHashCode();
                return hashCode.ToString("00000000") + Id;
            }
        }

		#endregion

		#region Identity Methods

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			return userIdentity;
		}

		#endregion
	}
}
