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
using LanAdeptData.DAL.Maps;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptData.DAL.Canteen;

namespace LanAdeptData.DAL
{
	public class UnitOfWork : IUnitOfWork
	{
		public static UnitOfWork Current
		{
			get { return HttpContext.Current.Items["_UnitOfWork"] as UnitOfWork; }
		}

		private LanAdeptDataContext context;

		public LanAdeptDataContext Context
		{
			get { return context; }
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

		private GamerTagRepository gamerTagRepository;
		public GamerTagRepository GamerTagRepository
		{
			get
			{
				if (gamerTagRepository == null)
				{
					gamerTagRepository = new GamerTagRepository(context);
				}
				return gamerTagRepository;
			}
		}

		private DemandeRepository demandeRepository;
		public DemandeRepository DemandeRepository
		{
			get
			{
				if (demandeRepository == null)
				{
					demandeRepository = new DemandeRepository(context);
				}
				return demandeRepository;
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

		private TileRepository tileRepository;
		public TileRepository TileRepository
		{
			get
			{
				if (tileRepository == null)
				{
					tileRepository = new TileRepository(context);
				}
				return tileRepository;
			}
		}

		private GuestRepository guestRepository;
		public GuestRepository GuestRepository
		{
			get
			{
				if (guestRepository == null)
				{
					guestRepository = new GuestRepository(context);
				}
				return guestRepository;
			}
		}

		private ProductRepository productRepository;
		public ProductRepository ProductRepository
		{
			get
			{
				if (productRepository == null)
				{
					productRepository = new ProductRepository(context);
				}
				return productRepository;
			}
		}

		private ItemRepository itemRepository;
		public ItemRepository ItemRepository
		{
			get
			{
				if (itemRepository == null)
				{
					itemRepository = new ItemRepository(context);
				}
				return itemRepository;
			}
		}

		private OrderRepository orderRepository;
		public OrderRepository OrderRepository
		{
			get
			{
				if (orderRepository == null)
				{
					orderRepository = new OrderRepository(context);
				}
				return orderRepository;
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
