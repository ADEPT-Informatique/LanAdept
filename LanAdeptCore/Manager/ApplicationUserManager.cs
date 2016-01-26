using LanAdeptData.DAL;
using LanAdeptData.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Manager
{
	public class ApplicationUserManager : UserManager<User>
	{
		public ApplicationUserManager(IUserStore<User> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<User>(UnitOfWork.Current.Context));

			// Email validation
			manager.UserValidator = new UserValidator<User>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Password validation
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 4,
				RequireNonLetterOrDigit = false,
				RequireDigit = true,
				RequireLowercase = false,
				RequireUppercase = false,
			};

			// Configurer les valeurs par défaut du verrouillage de l'utilisateur
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Inscrire les fournisseurs d'authentification à 2 facteurs. Cette application utilise le téléphone et les e-mails comme procédure de réception de code pour confirmer l'utilisateur
			// Vous pouvez écrire votre propre fournisseur et le connecter ici.
			manager.RegisterTwoFactorProvider("Code téléphonique ", new PhoneNumberTokenProvider<User>
			{
				MessageFormat = "Votre code de sécurité est {0}"
			});
			manager.RegisterTwoFactorProvider("Code d'e-mail", new EmailTokenProvider<User>
			{
				Subject = "Code de sécurité",
				BodyFormat = "Votre code de sécurité est {0}"
			});

			//manager.EmailService = new EmailService();
			//manager.SmsService = new SmsService();

			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}
}
