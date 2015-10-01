using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LanAdeptData.DAL.Generic;
using LanAdeptData.DAL.Places;
using LanAdeptData.DAL.Users;
using LanAdeptData.DAL.Tournaments;
using LanAdeptData.DAL.Settings;

namespace LanAdeptData.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private LanAdeptDataContext context;

        public static UnitOfWork Current
        {
            get { return HttpContext.Current.Items["_UnitOfWork"] as UnitOfWork; }
        }

        public UnitOfWork()
        {
            context = new LanAdeptDataContext();
        }

        private UserRepository userRepository;
        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(context);

                return userRepository;
            }
        }

        private RoleRepository roleRepository;
        public RoleRepository RoleRepository
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = new RoleRepository(context);

                return roleRepository;
            }
        }

        private PermissionRepository permissionRepository;
        public PermissionRepository PermissionRepository
        {
            get
            {
                if (permissionRepository == null)
                    permissionRepository = new PermissionRepository(context);

                return permissionRepository;
            }
        }

        private LoginHistoryRepository loginHistoryRepository;
        public LoginHistoryRepository LoginHistoryRepository
        {
            get
            {
                if (loginHistoryRepository == null)
                    loginHistoryRepository = new LoginHistoryRepository(context);

                return loginHistoryRepository;
            }
        }

        private PlaceRepository placeRepository;
        public PlaceRepository PlaceRepository
        {
            get
            {
                if (placeRepository == null)
                    placeRepository = new PlaceRepository(context);

                return placeRepository;
            }
        }

        private PlaceSectionRepository placeSectionRepository;
        public PlaceSectionRepository PlaceSectionRepository
        {
            get
            {
                if (placeSectionRepository == null)
                    placeSectionRepository = new PlaceSectionRepository(context);

                return placeSectionRepository;
            }
        }

        private ReservationRepository reservationRepository;
        public ReservationRepository ReservationRepository
        {
            get
            {
                if (reservationRepository == null)
                    reservationRepository = new ReservationRepository(context);

                return reservationRepository;
            }
        }

		#region Tournaments Repo
		private TournamentRepository tournamentRepository;
        public TournamentRepository TournamentRepository
        {
            get
            {
                if (tournamentRepository == null)
                {
                    tournamentRepository = new TournamentRepository(context);
                }
                return tournamentRepository;
            }
        }

        private TeamRepository teamRepository;
        public TeamRepository TeamRepository
        {
            get
            {
                if (teamRepository == null)
                {
                    teamRepository = new TeamRepository(context);
                }
                return teamRepository;
            }
        }

		private GameRepository gameRepository;
		public GameRepository GameRepository
		{
			get
			{
				if (gameRepository == null)
				{
					gameRepository = new GameRepository(context);
				}
				return gameRepository;
			}
		}

		private GamerRepository gamerRepository;
		public GamerRepository GamerRepository
		{
			get
			{
				if (gamerRepository == null)
				{
					gamerRepository = new GamerRepository(context);
				}
				return gamerRepository;
			}
		}
		#endregion

		private SettingRepository settingRepository;
		public SettingRepository SettingRepository
		{
			get
			{
				if (settingRepository == null)
				{
					settingRepository = new SettingRepository(context);
				}
				return settingRepository;
			}
		}

        private MapRepository mapRepository;
        public MapRepository MapRepository
        {
            get
            {
                if (mapRepository == null)
                {
                    mapRepository = new MapRepository(context);
                }
                return mapRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #region IDisposable

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
