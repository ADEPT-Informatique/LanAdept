using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptCore.Service
{
	public static class PlaceService
	{
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
			return !place.IsFree && place.LastReservation.User == UserService.GetLoggedInUser();
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
			try
			{
				if(!place.IsFree)
					return new BaseResult() { Message = "Désolé, cette place est déjà occupée ou réservée. Vous ne pouvez pas la réservée.", HasError = true };

				Reservation reservation = new Reservation();
				reservation.CreationDate = DateTime.Now;
				reservation.User = user;
				reservation.Place = place;

				if (user.LastReservation != null && !user.LastReservation.IsCancelled)
					CancelUserReservation(user);

				UnitOfWork.Current.PlaceHistoryRepository.Insert(reservation);
				UnitOfWork.Current.Save();

				return new BaseResult();
			}
			catch 
			{
			#if DEBUG
				throw;
			#endif
			}

			return new BaseResult() { Message = "Désolé, ne erreur est survenue. Merci de réessayer dans quelques instants.", HasError = true };
		}

		public static void CancelReservation(Place place)
		{
			if (!place.LastReservation.IsCancelled)
			{
				place.LastReservation.CancellationDate = DateTime.Now;
				UnitOfWork.Current.PlaceHistoryRepository.Update(place.LastReservation);
				UnitOfWork.Current.Save();
			}
		}

		public static void CancelUserReservation(User user)
		{
			if (!user.LastReservation.IsCancelled)
			{
				user.LastReservation.CancellationDate = DateTime.Now;
				UnitOfWork.Current.PlaceHistoryRepository.Update(user.LastReservation);
				UnitOfWork.Current.Save();
			}
		}

	}
}
