using System;
using System.Collections.Generic;
using System.Linq;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Users;
using System.Net;
using System.IO;
using System.Text;

namespace LanAdeptCore.Service
{
	public static class ReservationService
	{
		private static UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		/// <summary>
		/// Determine wether the currently connected user has reserved a place or not
		/// </summary>
		/// <returns>True if the user has a reserved place, else false</returns>
		public static bool HasUserPlace()
		{
			return HasUserPlace(UserService.GetLoggedInUser());
		}

		/// <summary>
		/// Determine wether a user has reserved a place or not
		/// </summary>
		/// <param name="user">The user to check</param>
		/// <returns>True if the user has a reserved place, else false</returns>
		public static bool HasUserPlace(User user)
		{
			return user != null
				&& user.LastReservation != null
				&& !user.LastReservation.IsCancelled
				&& user.LastReservation.LeavingDate == null;
		}


		/// <summary>
		/// Determine wether the place is the currently connected user's place
		/// </summary>
		/// <param name="place">Place to verify</param>
		/// <returns>True if the place is the user's place, else false</returns>
		public static bool IsUserPlace(Place place)
		{
			return IsUserPlace(place, UserService.GetLoggedInUser());
		}

		/// <summary>
		/// Determine wether the place is the a specific user's place
		/// </summary>
		/// <param name="place">Place to verify</param>
		/// <returns>True if the place is the user's place, else false</returns>
		public static bool IsUserPlace(Place place, User user)
		{
			return !place.IsFree
				&& place.LastReservation.User != null
				&& place.LastReservation.User == UserService.GetLoggedInUser();
		}

		/// <summary>
		/// Get the currently logged in user's place
		/// </summary>
		/// <returns>The place of the user, or null if he doesn't have one</returns>
		public static Place GetUserPlace()
		{
			return GetUserPlace(UserService.GetLoggedInUser());
		}

		/// <summary>
		/// Get the user's place
		/// </summary>
		/// <param name="user">User to verify</param>
		/// <returns>The place of the user, or null if he doesn't have one</returns>
		public static Place GetUserPlace(User user)
		{
			if (user == null || user.LastReservation == null)
				return null;

			if (HasUserPlace(user))
			{
				return user.LastReservation.Place;
			}

			return null;
		}

		/// <summary>
		/// Reserve a place for the currently connected user. If the user already has a place reserved,
		/// this method will cancel his reservation and change it for the selected place
		/// </summary>
		/// <param name="place">Place to reserve</param>
		public static BaseResult ReservePlace(Place place)
		{
			return ReservePlace(place, UserService.GetLoggedInUser());
		}

		/// <summary>
		/// Reserve a place for a specific user. If the user already has a place reserved,
		/// this method will cancel his reservation and change it for the selected place
		/// </summary>
		/// <param name="place">Place to reserve</param>
		/// <param name="user">User that is reserving the place</param>
		public static BaseResult ReservePlace(Place place, User user)
		{
			if (!place.IsFree)
				return new BaseResult() { Message = "Désolé, cette place est déjà occupée ou réservée. Vous ne pouvez pas la réservée.", HasError = true };

			if (user.LastReservation != null && !user.LastReservation.IsCancelled)
				CancelUserReservation(user);
            var request = (HttpWebRequest)WebRequest.Create("https://app.seats.io/api/book");
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json =
                  "{\'objects\': " + "[{\'objectId\': " + "\'" + place.SeatsId + "\', \'extraData\': { \'name\': \'" + user.CompleteName + "\'}}]" + "," +
                    "\'eventKey\': " + "\'" + uow.SettingRepository.GetCurrentSettings().EventKeyId.Replace(" ", "") + "\'" + "," +
                    "\'secretKey\': " + "\'" + uow.SettingRepository.GetCurrentSettings().SecretKeyId.Replace(" ","") + "\'}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var response = streamReader.ReadToEnd();
            }
            Reservation reservation = new Reservation();
			reservation.CreationDate = DateTime.Now;
			reservation.User = user;
			reservation.Place = place;

			uow.ReservationRepository.Insert(reservation);
			uow.Save();

			return new BaseResult();
		}

		/// <summary>
		/// Reserve a place for a guest.
		/// </summary>
		/// <param name="place">Place to reserve</param>
		/// <param name="user">Guest that is reserving the place</param>
		public static BaseResult ReservePlace(Place place, string guestName)
		{
			if (!place.IsFree)
				return new BaseResult() { Message = "Désolé, cette place est déjà occupée ou réservée. Vous ne pouvez pas la réservée.", HasError = true };

			if (string.IsNullOrWhiteSpace(guestName))
				return new BaseResult() { Message = "Le nom de l'invité ne peut pas être vide.", HasError = true };
            var request = (HttpWebRequest)WebRequest.Create("https://app.seats.io/api/book");
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json =
                  "{\'objects\': " + "[{\'objectId\':" + "\'" + place.SeatsId + "\', \'extraData\': { \'name\': \'" + guestName + "\'}}]" + "," +
                    "\'eventKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().EventKeyId.Replace(" ", "") + "\'" + "," +
                    "\'secretKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().SecretKeyId.Replace(" ", "") + "\'}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var response = streamReader.ReadToEnd();
            }
            Guest guest = new Guest();
			guest.CompleteName = guestName;

			Reservation reservation = new Reservation();
			reservation.CreationDate = DateTime.Now;
			reservation.Guest = guest;
			reservation.Place = place;

			uow.GuestRepository.Insert(guest);
			uow.ReservationRepository.Insert(reservation);
			uow.Save();

			return new BaseResult();
		}

		public static BaseResult UserArrived(Place place)
		{
			if (place.IsFree)
				return new BaseResult() { Message = "La place <strong>" + place + "</strong> n'est pas réservé!", HasError = true };
			if (place.LastReservation.ArrivalDate != null)
				return new BaseResult() { Message = "La place <strong>" + place + "</strong> est déja occupé!", HasError = true };

			Reservation reservation = place.LastReservation;
			reservation.ArrivalDate = DateTime.Now;
			uow.ReservationRepository.Update(reservation);
			uow.Save();

			return new BaseResult();
		}

		/// <summary>
		/// Cancel the reservation any user might have for a specific place
		/// </summary>
		/// <param name="place">Place to cancel the reservation</param>
		public static void CancelReservation(Place place)
		{
			if (!place.LastReservation.IsCancelled)
			{
                var request = (HttpWebRequest)WebRequest.Create("https://app.seats.io/api/release");
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json =
                      "{\'objects\': " + "[\'" + place.SeatsId + "\']" + "," +
                        "\'eventKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().EventKeyId.Replace(" ", "") + "\'" + "," +
                        "\'secretKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().SecretKeyId.Replace(" ", "") + "\'}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                }
                place.LastReservation.CancellationDate = DateTime.Now;
				uow.ReservationRepository.Update(place.LastReservation);
				uow.Save();
			}
		}

		/// <summary>
		/// Cancel the currently connected user's reservation
		/// </summary>
		public static void CancelUserReservation()
		{
			CancelUserReservation(UserService.GetLoggedInUser());
		}

		/// <summary>
		/// Cancel a user's reservation
		/// </summary>
		/// <param name="user">User to whom the reservation will be cancelled</param>
		public static void CancelUserReservation(User user)
		{
			if (!user.LastReservation.IsCancelled)
			{
                var request = (HttpWebRequest)WebRequest.Create("https://app.seats.io/api/release");
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json =
                      "{\'objects\': " + "[\'" + user.LastReservation.Place.SeatsId + "\']" + "," +
                        "\'eventKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().EventKeyId.Replace(" ", "") + "\'" + "," +
                        "\'secretKey\' :" + "\'" + uow.SettingRepository.GetCurrentSettings().SecretKeyId.Replace(" ", "") + "\'}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                }
                user.LastReservation.CancellationDate = DateTime.Now;
				uow.ReservationRepository.Update(user.LastReservation);
				uow.Save();
			}
		}
	}
}
