using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptCore.Service
{
	public static class UserService
	{
		private const int SALT_LENGTH = 20;

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
