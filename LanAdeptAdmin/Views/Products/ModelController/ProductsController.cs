using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model.Canteen;

namespace LanAdeptAdmin.Views.Products.ModelController
{
	public class ProductsController : Controller
	{
		//private LanAdeptDataContext db = new LanAdeptDataContext();
		private UnitOfWork uow = UnitOfWork.Current;

		// GET: Products
		public ActionResult Index()
		{
			return View(uow.ProductRepository.Get().ToList());
		}

		// GET: Products/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = uow.ProductRepository.GetByID(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		// GET: Products/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ProductID,Name,Price,InStock,IsVisible,ImagePath")] Product product)
		{
			if (ModelState.IsValid)
			{
				uow.ProductRepository.Insert(product);
				uow.Save();
				return RedirectToAction("Index");
			}

			return View(product);
		}

		// GET: Products/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = uow.ProductRepository.GetByID(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ProductID,Name,Price,InStock,IsVisible,ImagePath")] Product product)
		{
			if (ModelState.IsValid)
			{
				uow.ProductRepository.Update(product);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View(product);
		}

		// GET: Products/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Product product = uow.ProductRepository.GetByID(id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			uow.ProductRepository.Delete(id);
			uow.Save();
			return RedirectToAction("Index");
		}
	}
}
