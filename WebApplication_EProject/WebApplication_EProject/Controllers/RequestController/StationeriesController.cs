using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using PagedList;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.Request;

namespace WebApplication_EProject.Controllers.RequestController
{
	[Authorize(Roles = "root, manager, Office_supplies_stationery")]
	public class StationeriesController : Controller
	{
		private ModelContext db = new ModelContext();

		public void Statuscheck()
		{
			var account = User.Identity.GetUserName();
			var Check_Email = db.Employees.FirstOrDefault(x => x.Email == account);
			if (Check_Email.Status == 0)
			{
				ViewBag.haha = "Account currently disabled";
				FormsAuthentication.SignOut();
			}
		}
		// GET: Stationeries
		public ActionResult Index(string SearchString, int? Page_No)
		{
			Statuscheck();
			ViewBag.SortingByFullName = String.IsNullOrEmpty(SearchString) ? "FullName_Sort" : "";

			var item = from a in db.Stationeries select a;
			if (!String.IsNullOrEmpty(SearchString))
			{
				item = item.Where(a => a.productName.Contains(SearchString));
			}

			item = item.OrderBy(a => a.producer);
			int Page_Size = 4;
			int No_Of_Page = (Page_No ?? 1);
			//var customers = db.Customers.Include(c => c.Class);
			return View(item.ToPagedList(No_Of_Page, Page_Size));
		}

		// GET: Stationeries/Details/5
		public ActionResult Details(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Stationery stationeries = db.Stationeries.Find(id);
			if (stationeries == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(stationeries);
		}

		// GET: Stationeries/Create
		public ActionResult Create()
		{
			Statuscheck();
			return View();
		}

		// POST: Stationeries/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "productID,productName,producer")] Stationery stationeries, HttpPostedFileBase productImage)
		{
			Statuscheck();
			if (productImage != null)
			{

				var supportedTypes = new[] { "jpg", "jpeg", "png" };
				var fileExt = System.IO.Path.GetExtension(productImage.FileName).Substring(1);

				if (!supportedTypes.Contains(fileExt))
				{
					ViewBag.ErrorMessage = "File Extension Is InValid - Only Upload jpg/jpeg/png File";
					return View();
				}
				string FileName = System.IO.Path.GetFileName(productImage.FileName);
				string URLLink = Server.MapPath("~/images/" + FileName);
				stationeries.productImage = FileName;
				WebImage imgSize = new WebImage(productImage.InputStream);
				imgSize.Resize(100, 100);
				imgSize.Save(URLLink);


			}
			if (ModelState.IsValid)
			{
				db.Stationeries.Add(stationeries);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(stationeries);
		}

		// GET: Stationeries/Edit/5
		public ActionResult Edit(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Stationery stationeries = db.Stationeries.Find(id);
			if (stationeries == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(stationeries);
		}

		// POST: Stationeries/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "productID,productName,producer")] Stationery stationeries, HttpPostedFileBase productImage)
		{
			Statuscheck();
			if (productImage != null)
			{

				var supportedTypes = new[] { "jpg", "jpeg", "png" };
				var fileExt = System.IO.Path.GetExtension(productImage.FileName).Substring(1);

				if (!supportedTypes.Contains(fileExt))
				{
					ViewBag.ErrorMessage = "File Extension Is InValid - Only Upload jpg/jpeg/png File";
					return View();
				}
				string FileName = System.IO.Path.GetFileName(productImage.FileName);
				string URLLink = Server.MapPath("~/images/" + FileName);

				stationeries.productImage = FileName;
				WebImage imgSize = new WebImage(productImage.InputStream);
				imgSize.Resize(100, 100);
				imgSize.Save(URLLink);

				
			}
			if (ModelState.IsValid)
			{

				db.Entry(stationeries).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(stationeries);
		}

		// GET: Stationeries/Delete/5
		public ActionResult Delete(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Stationery stationeries = db.Stationeries.Find(id);
			if (stationeries == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(stationeries);
		}

		// POST: Stationeries/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Statuscheck();
			Stationery stationeries = db.Stationeries.Find(id);

			//xóa bảng logs
			var request_check = db.Requests.FirstOrDefault(r => r.productID == id);
			if (request_check != null)
			{
				var log_check = from l in db.Logs where l.Employee_ID == request_check.Employee_ID select l;
				db.Logs.RemoveRange(log_check);

				//xóa bảng yêu cầu
				var requests_check = from r in db.Requests where r.Employee_ID == request_check.Employee_ID select r;
				db.Requests.RemoveRange(requests_check);
			}

			//xóa văn phòng phẩm
			db.Stationeries.Remove(stationeries);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
