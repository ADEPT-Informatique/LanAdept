using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptCore.Service
{
	public static class UserService
	{
		private const int SALT_LENGTH = 20;
		private const string ERROR_MESSAGE_INVALID_LOGIN = "L'adresse e-mail et le mot de passe saisis ne correspondent pas.";
		private const string ERROR_MESSAGE_UNCONFIRMED_LOGIN = "Votre compte n'est pas actif. Vous devez confirmer votre adresse email avant de pouvoir vous connecter.";

		/// <summary>
		/// Create a user with a salt and his password hashed
		/// </summary>
		/// <param name="email">User email</param>
		/// <param name="password">User password unhashed</param>
		/// <returns>User object with his random salt and hashed password</returns>
		public static User CreateUser(string email, string password, string completeName)
		{
			User user = new User();

			user.Email = email;
			user.Salt = CreateSalt(SALT_LENGTH);
			user.Password = HashPassword(password, user.Salt);
			user.RoleID = UnitOfWork.Current.RoleRepository.GetUnconfirmedRole().RoleID;
			user.CompleteName = completeName;

			return user;
		}

		/// <summary>
		/// Determine if a user login input is valid
		/// </summary>
		/// <param name="email">Email inputed by the user</param>
		/// <param name="password">Password inputed by the user</param>
		/// <returns>An object containing if the user logged in successfuly, and if he did, the user, or else, the reason why he didn't succeed</returns>
		public static TryLoginResult TryLogin(string email, string password, bool rememberUser)
		{
			User realUser = UnitOfWork.Current.UserRepository.GetUserByEmail(email);
			if (realUser == null)
				return new TryLoginResult() { HasSucceeded = false, Reason = ERROR_MESSAGE_INVALID_LOGIN };

			string hashedInputPassword = HashPassword(password, realUser.Salt);

			if (realUser.Password != hashedInputPassword)
				return new TryLoginResult() { HasSucceeded = false, Reason = ERROR_MESSAGE_INVALID_LOGIN };

			if(realUser.Role.IsUnconfirmedRole)
				return new TryLoginResult() { HasSucceeded = false, Reason = ERROR_MESSAGE_UNCONFIRMED_LOGIN };

			// À partir de ce point, la connexion est réussie
			realUser.LoginHistories.Add(UnitOfWork.Current.LoginHistoryRepository.CreateHistoryFor(realUser));
			UnitOfWork.Current.UserRepository.Update(realUser);
			UnitOfWork.Current.Save();

			// Set le cookie de connexion avec un long timeout si le user veux rester connecté
			int timeout = rememberUser ? 525600 : 30; //525600 = 1 an
			var ticket = new FormsAuthenticationTicket(realUser.Email, rememberUser, timeout);
			string encrypted = FormsAuthentication.Encrypt(ticket);
			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
			cookie.Expires = System.DateTime.Now.AddMinutes(timeout);//My Line
			HttpContext.Current.Response.Cookies.Add(cookie);			

			return new TryLoginResult() { HasSucceeded = true, User = realUser };
		}

		/// <summary>
		/// Disconnect the user from the website
		/// </summary>
		public static void Logout() {
			FormsAuthentication.SignOut();
		}

		public static string EncryptSalt(string salt)
		{
			byte[] saltByteArray = System.Text.Encoding.Unicode.GetBytes(salt);
			salt = System.Convert.ToBase64String(saltByteArray);
			return HttpUtility.UrlEncode(salt);
		}

		public static string DecryptSalt(string encryptedSalt)
		{
			encryptedSalt = HttpUtility.UrlDecode(encryptedSalt);
			byte[] saltByteArray = System.Convert.FromBase64String(encryptedSalt);
			return System.Text.Encoding.Unicode.GetString(saltByteArray);
		}

		public static User GetLoggedInUser()
		{
			if (!HttpContext.Current.User.Identity.IsAuthenticated)
				return null;

			return UnitOfWork.Current.UserRepository.GetUserByEmail(HttpContext.Current.User.Identity.Name);
		}

		/* =============== PRIVÉ =============== */

		private static string HashPassword(string password, string salt)
		{
			HashAlgorithm algorithm = SHA256.Create();
			byte[] hashedPassword = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt));

			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashedPassword)
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		/// <summary>
		/// Create a random salt for password
		/// </summary>
		/// <param name="size">Size of the desired salt</param>
		private static string CreateSalt(int size)
		{
			RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
			byte[] salt = new byte[size];
			csprng.GetBytes(salt);

			return Convert.ToBase64String(salt);
		}
	}
}
