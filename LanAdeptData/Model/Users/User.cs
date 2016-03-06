using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Tournaments;
using LanAdeptData.Model.Canteen;

namespace LanAdeptData.Model.Users
{
	public class User : IdentityUser
	{
		[Required]
		[Display(Name = "Nom complet")]
		public string CompleteName { get; set; }

		public string Barcode { get; set; }

		#region Navigation properties

		public virtual ICollection<Reservation> Reservations { get; set; }

		public virtual ICollection<Team> Teams { get; set; }

		public virtual ICollection<Order> Orders { get; set; }

		#endregion

		#region Calculated properties

		public Reservation LastReservation
		{
			get
			{
				return Reservations.OrderBy(r => r.CreationDate).LastOrDefault();
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
