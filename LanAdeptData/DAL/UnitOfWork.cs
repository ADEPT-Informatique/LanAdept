using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;

namespace LanAdeptData.DAL
{
	public class UnitOfWork : IUnitOfWork
	{
		private LanAdeptDataContext context;

		public UnitOfWork()
		{
			context = new LanAdeptDataContext();
		}

		private TestRepository testRepository;

		public TestRepository TestRepository
		{
			get
			{
				if (testRepository == null)
					testRepository = new TestRepository(context);

				return testRepository;
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
