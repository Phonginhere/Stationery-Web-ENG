using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.Request;

namespace WebApplication_EProject.Controllers.RequestController
{
	[Authorize]
	public class RequestsController : Controller
	{
		String ck;
		String notify_successful = "Update succesfully";
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

		public void Logcheck(DateTime DateAdd, int role_check, int employ_check)
		{
			Statuscheck();
			var check_request = db.Requests.FirstOrDefault(r => r.DateAdd == DateAdd);

			Log logg = new Log();
			logg.Request_Id = check_request.Request_Id;
			logg.Role_ID = role_check;
			logg.Employee_ID = employ_check;
			logg.LogName = "Create request";
			logg.LogDate = DateTime.Now;
			db.Logs.Add(logg);
			db.SaveChanges();
		}
		public void Logcheck_id(int id, String ck)
		{
			Statuscheck();
			//begin log request stationary
			var check_request = db.Requests.FirstOrDefault(r => r.Request_Id == id);
			var username = User.Identity.GetUserName();
			var user_check = db.Employees.FirstOrDefault(n => n.Email == username);

			Log logg = new Log();
			logg.Request_Id = check_request.Request_Id;
			logg.Role_ID = user_check.Role_ID;
			logg.Employee_ID = user_check.Employee_ID;
			logg.LogDate = DateTime.Now;
			
			if (ck == "Delete")
			{
				logg.LogName = "Cancel Request";
			}
			else if (ck == "-1")
			{
				logg.LogName = "Refused request";
			}
			else if (ck == "1")
			{
				logg.LogName = "Approved from Head";
			}
			else if (ck == "2")
			{
				logg.LogName = "Manager Approved";
			}
			else if (ck == "Pause")
			{
				logg.LogName = "Pause";
			}
			else if (ck == "Get Product")
			{
				logg.LogName = "Get Product";
			}
			else if (ck == "Returns Product")
			{
				logg.LogName = "Returns Product";
			}
			else if (ck == "Get the Product Back")
			{
				logg.LogName = "Get the Product Back";
			}
			else if (ck == "Provide Unit Price")
			{
				logg.LogName = "Provide Unit Price";
			}
			else if (ck == "Pause check money")
			{
				logg.LogName = "Pause check money";
			}
			else if (ck == "Check price successfully")
			{
				logg.LogName = "Check price successfully";
			}
			else if (ck == "Refuse Price")
			{
				logg.LogName = "Refuse Price";
			}
			else if (ck == "Send Money")
			{
				logg.LogName = "Send Money";
			}
			else if (ck == "Buy Product")
			{
				logg.LogName = "Buy Product";
			}
			else if (ck == "Human Department get product")
			{
				logg.LogName = "Human Department get product";
			}
			db.Logs.Add(logg);

			//end log request stationary
		}

		// GET: Requests
		public ActionResult Index(String Status_Check, String Role_ID)
		{
			Statuscheck();
			if (Status_Check == null || Role_ID == null)
			{
				goto Here;
			}
			if (Status_Check != "" && Role_ID != "")
			{
				int check_status = Convert.ToInt32(Status_Check);
				int check_role = Convert.ToInt32(Role_ID);
				var requestss = from r in db.Requests where r.Role_ID == check_role && r.Status == check_status select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(requestss);
			}

			if (Status_Check != "")
			{

				int check_status = Convert.ToInt32(Status_Check);
				var requestss = from r in db.Requests where r.Status == check_status select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(requestss);
			}
			else if (Role_ID != "")
			{
				int check_role = Convert.ToInt32(Role_ID);
				var requestss = from r in db.Requests where r.Role_ID == check_role select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(requestss);
			}

		Here: //come here

			if (TempData["app_false"] != null)
			{
				ViewBag.app_false = TempData["app_false"];
			}
			if (TempData["Notify"] != null)
			{
				ViewBag.Notify = TempData["Notify"];
			}
			var requests = db.Requests.Include(r => r.Employee).Include(r => r.Role).Include(r => r.Stationery);

			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			return View(requests);
		}

		[Authorize(Roles = "root, manager, Office_supplies_stationery, human_resources_department, accounting_department")]
		public ActionResult Index_Group_Product(String Status_Check, String Role_ID)
		{
			Statuscheck();
			if (Status_Check == null || Role_ID == null)
			{
				goto Here;
			}
			if (Status_Check != "" && Role_ID != "")
			{
				int check_status = Convert.ToInt32(Status_Check);
				int check_role = Convert.ToInt32(Role_ID);
				var request_list_haha = from r in db.Requests
										where r.Status == check_status && r.Role_ID == check_role
										group r by new { r.Role.RoleName, r.Stationery.productName, r.Unit, r.Price, r.Status, r.Pause, r.Note } into g
										select new Group<string, Request>
										{
											KeyRole = g.Key.RoleName,
											KeyProduct = g.Key.productName,
											KeyUnit = g.Key.Unit,
											KeyQuantity = g.Sum(l => l.Quantity).ToString(),
											KeyPrice = g.Key.Price.ToString(),
											KeyStatus = g.Key.Status.ToString(),
											KeyPause = g.Key.Pause.ToString(),
											KeyNote = g.Key.Note,
											Values = g
										};
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(request_list_haha.ToList());
			}

			if (Status_Check != "")
			{

				int check_status = Convert.ToInt32(Status_Check);
				var request_list_haha = from r in db.Requests
										where r.Status == check_status
										group r by new { r.Role.RoleName, r.Stationery.productName, r.Unit, r.Price, r.Status, r.Pause, r.Note } into g
										select new Group<string, Request>
										{
											KeyRole = g.Key.RoleName,
											KeyProduct = g.Key.productName,
											KeyUnit = g.Key.Unit,
											KeyQuantity = g.Sum(l => l.Quantity).ToString(),
											KeyPrice = g.Key.Price.ToString(),
											KeyStatus = g.Key.Status.ToString(),
											KeyPause = g.Key.Pause.ToString(),
											KeyNote = g.Key.Note,
											Values = g
										};
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(request_list_haha.ToList());
			}
			else if (Role_ID != "")
			{
				int check_role = Convert.ToInt32(Role_ID);
				var request_list_haha = from r in db.Requests
										where r.Role_ID == check_role
										group r by new { r.Role.RoleName, r.Stationery.productName, r.Unit, r.Price, r.Status, r.Pause, r.Note } into g
										select new Group<string, Request>
										{
											KeyRole = g.Key.RoleName,
											KeyProduct = g.Key.productName,
											KeyUnit = g.Key.Unit,
											KeyQuantity = g.Sum(l => l.Quantity).ToString(),
											KeyPrice = g.Key.Price.ToString(),
											KeyStatus = g.Key.Status.ToString(),
											KeyPause = g.Key.Pause.ToString(),
											KeyNote = g.Key.Note,
											Values = g
										};
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(request_list_haha.ToList());
			}

		Here: //Jump to here

			//begin notify
			if (TempData["app_false"] != null)
			{
				ViewBag.app_false = TempData["app_false"];
			}
			if (TempData["no_zero"] != null)
			{
				ViewBag.no_zero = TempData["no_zero"];
			}
			if (TempData["Notify"] != null)
			{
				ViewBag.Notify = TempData["Notify"];
			}
			//end notify

			//display
			var request_list = from r in db.Requests
							   where r.Status == 2 || r.Status == 3 || r.Status == 4 || r.Status == 5 || r.Status == 6
							   group r by new { r.Role.RoleName, r.Stationery.productName, r.Unit, r.Price, r.Status, r.Pause, r.Note } into g
							   select new Group<string, Request>
							   {
								   KeyRole = g.Key.RoleName,
								   KeyProduct = g.Key.productName,
								   KeyUnit = g.Key.Unit,
								   KeyQuantity = g.Sum(l => l.Quantity).ToString(),
								   KeyPrice = g.Key.Price.ToString(),
								   KeyStatus = g.Key.Status.ToString(),
								   KeyPause = g.Key.Pause.ToString(),
								   KeyNote = g.Key.Note,
								   Values = g
							   };

			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			return View(request_list.ToList());
		}

		// GET: Requests/Details/5
		public ActionResult Details(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}

		// GET: Requests/Create
		public ActionResult Create()
		{
			Statuscheck();
			ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name");
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			ViewBag.productID = new SelectList(db.Stationeries, "productID", "productName");
			return View();
		}

		// POST: Requests/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Request_Id,Employee_ID,Role_ID,productID,Unit,Quantity,Purpose,DateAdd,Status,Pause,Note")] Request request)
		{
			Statuscheck();
			if (ModelState.IsValid)
			{
				var check_username = User.Identity.GetUserName();

				var check_user = db.Employees.Where(c => c.Email == check_username);

				int role_check = (int)check_user.Select(r => r.Role_ID).First();
				int employ_check = (int)check_user.Select(r => r.Employee_ID).First();
				DateTime now = DateTime.Now;
				String Datenow = Convert.ToString(now);
				if (role_check == 2)
				{
					TempData["Notify"] = "Create Succesfully";
					request.Employee_ID = employ_check;
					request.Role_ID = role_check;
					request.DateAdd = Convert.ToDateTime(Datenow);
					request.Status = 1;
					request.Price = 0;
					request.Note = "";
					db.Requests.Add(request);
					db.SaveChanges();
					Logcheck(Convert.ToDateTime(Datenow), role_check, employ_check);

					return RedirectToAction("Index");
				}
				TempData["Notify"] = "Create Succesfully";
				request.Employee_ID = employ_check;
				request.Role_ID = role_check;
				request.DateAdd = Convert.ToDateTime(Datenow);
				request.Price = 0;
				request.Note = "";
				db.Requests.Add(request);
				db.SaveChanges();
				Logcheck(Convert.ToDateTime(Datenow), role_check, employ_check);
				return RedirectToAction("Index");
			}

			ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", request.Employee_ID);
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", request.Role_ID);
			ViewBag.productID = new SelectList(db.Stationeries, "productID", "productName", request.productID);
			return View(request);
		}


		//Get: Head of Production / 3 department check request info
		[Authorize(Roles = "head_of_production, head_of_humanresources_officesupplies_accountingdepartment, root")]
		public ActionResult Head_Check(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			var user_check = User.Identity.GetUserName();
			var user_role_check = db.Employees.FirstOrDefault(ur => ur.Email == user_check);
			int role_check = (int)request.Role_ID;
			if (user_role_check.Role_ID == 8)
			{

				if (role_check == 6 || role_check == 7 || role_check == 1)
				{
					TempData["app_false"] = "It is not allowed to check the head of production and production staff ";
					return RedirectToAction("Index");
				}
			}
			else if (user_role_check.Role_ID == 6)
			{
				if (role_check == 3 || role_check == 4 || role_check == 5 || role_check == 8)
				{
					TempData["app_false"] = "It is not allowed to check human resources, Office supplies, accountants department and heads of those 3 departments ";
					return RedirectToAction("Index");
				}
			}

			return View(request);
		}

		static String SqlCon = ConfigurationManager.ConnectionStrings["ModelContext"].ConnectionString;
		SqlConnection con = new SqlConnection(SqlCon);

		[Authorize(Roles = "head_of_production, head_of_humanresources_officesupplies_accountingdepartment, root")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Head_Check(int Request_Id, String Status, bool approve, String Note)
		{
			Statuscheck();
			if (Status == "1.5")
			{
				Status = "Pause";
			}
			string sqlQuery = "Update Requests SET Status = @Status, Pause = @Pause, Note = @Note where Request_Id = @Request_Id";

			if (Status == "0")
			{
				return RedirectToAction("Index");
			}
			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index");
			}
			//Pause
			ck = "Pause";
			if (Status == ck)
			{

				con.Open();
				SqlCommand cmd_tamhoan = new SqlCommand(sqlQuery, con);
				cmd_tamhoan.Parameters.AddWithValue("@Status", 0);
				cmd_tamhoan.Parameters.AddWithValue("@Pause", true);
				cmd_tamhoan.Parameters.AddWithValue("@Note", "");
				cmd_tamhoan.Parameters.AddWithValue("@Request_Id", Request_Id);
				int check_tamhoan = cmd_tamhoan.ExecuteNonQuery();
				con.Close();
				if (check_tamhoan == 1)
				{
					TempData["Notify"] = "Updated successfully";
					Logcheck_id(Request_Id, ck);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				return RedirectToAction("Index");
			}

			//từ chối
			if (Status == "-1")
			{
				if (Note == "")
				{
					TempData["app_false"] = "The reason for the refusal has not been specified";
					return RedirectToAction("Index");
				}
				con.Open();
				SqlCommand cmd_tuchoi = new SqlCommand(sqlQuery, con);
				cmd_tuchoi.Parameters.AddWithValue("@Status", Status);
				cmd_tuchoi.Parameters.AddWithValue("@Pause", false);
				cmd_tuchoi.Parameters.AddWithValue("@Note", Note);
				cmd_tuchoi.Parameters.AddWithValue("@Request_Id", Request_Id);
				int check_refused = cmd_tuchoi.ExecuteNonQuery();
				con.Close();
				if (check_refused == 1)
				{
					TempData["Notify"] = notify_successful;
					if (Status == "1")
					{
						Logcheck_id(Request_Id, "1");
						db.SaveChanges();
					}
					else if (Status == "-1")
					{
						Logcheck_id(Request_Id, "-1");
						db.SaveChanges();
					}
					return RedirectToAction("Index");
				}
			}
			// Confirm
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", Status);
			cmd.Parameters.AddWithValue("@Pause", false);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Request_Id", Request_Id);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Notify"] = notify_successful;
				if (Status == "1")
				{
					Logcheck_id(Request_Id, "1");
					db.SaveChanges();
				}
				else if (Status == "-1")
				{
					Logcheck_id(Request_Id, "-1");
					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		//Get: giám đốc check thông tin nhân viên yêu cầu
		[Authorize(Roles = "manager, root")]
		public ActionResult Manager_Check_Employee(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}

		[Authorize(Roles = "manager, root")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Manager_Check_Employee(int Request_Id, String Status, bool approve, String Note)
		{
			Statuscheck();
			if(Status == "1.5")
			{
				Status = "Pause";
			}
			string sqlQuery = "Update Requests SET Status = @Status, Pause = @Pause, Note = @Note where Request_Id = @Request_Id";

			if (Status == "0")
			{
				return RedirectToAction("Index");
			}
			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index");
			}
			//Pause
			ck = "Pause";
			if (Status == ck)
			{

				con.Open();
				SqlCommand cmd_tamhoan = new SqlCommand(sqlQuery, con);
				cmd_tamhoan.Parameters.AddWithValue("@Status", 1);
				cmd_tamhoan.Parameters.AddWithValue("@Pause", true);
				cmd_tamhoan.Parameters.AddWithValue("@Note", "");
				cmd_tamhoan.Parameters.AddWithValue("@Request_Id", Request_Id);
				int check_tamhoan = cmd_tamhoan.ExecuteNonQuery();
				con.Close();
				if (check_tamhoan == 1)
				{
					TempData["Notify"] = "Updated successfully";
					Logcheck_id(Request_Id, ck);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				return RedirectToAction("Index");
			}

			//từ chối
			if (Status == "-1")
			{
				if (Note == "")
				{
					TempData["app_false"] = "The reason for the refusal has not been specified";
					return RedirectToAction("Index_Group_Product");
				}
				con.Open();
				SqlCommand cmd_tuchoi = new SqlCommand(sqlQuery, con);
				cmd_tuchoi.Parameters.AddWithValue("@Status", Status);
				cmd_tuchoi.Parameters.AddWithValue("@Pause", false);
				cmd_tuchoi.Parameters.AddWithValue("@Note", Note);
				cmd_tuchoi.Parameters.AddWithValue("@Request_Id", Request_Id);
				int check_tuchoi = cmd_tuchoi.ExecuteNonQuery();
				con.Close();
				if (check_tuchoi == 1)
				{
					TempData["Notify"] = notify_successful;
					if (Status == "1")
					{
						Logcheck_id(Request_Id, "1");
						db.SaveChanges();
					}
					else if (Status == "-1")
					{
						Logcheck_id(Request_Id, "-1");
						db.SaveChanges();
					}
					return RedirectToAction("Index");
				}
			}
			// Confirm
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", Status);
			cmd.Parameters.AddWithValue("@Pause", false);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Request_Id", Request_Id);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Notify"] = notify_successful;
				if (Status == "2")
				{
					Logcheck_id(Request_Id, "2");
					db.SaveChanges();
				}
				else if (Status == "-1")
				{
					Logcheck_id(Request_Id, "-1");
					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		//begin group by 

		//Office_supplies_stationery provide unit price
		[Authorize(Roles = "Office_supplies_stationery, root")]
		public ActionResult Input_Price(string role, string product, string unit, string quantity, string price, string status, string note)
		{
			Statuscheck();
			if (role == null || product == null || unit == null || quantity == null || price == null || status == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.role = role;
			ViewBag.product = product;
			ViewBag.unit = unit;
			ViewBag.quantity = quantity;
			ViewBag.price = price;
			ViewBag.status = status;
			ViewBag.note = note;

			return View();
		}

		[Authorize(Roles = "Office_supplies_stationery, root")]
		[HttpPost]
		public ActionResult Input_Price(string RoleName, string productName, string Unit, string Quantity, string Price, string Status)
		{
			Statuscheck();
			if (Convert.ToInt64(Price) == 0)
			{
				TempData["no_zero"] = "Input number other than 0";
				return RedirectToAction("Index_Group_Product");
			}

			//thành công
			var rollid_check = db.Roles.FirstOrDefault(r => r.RoleName == RoleName);

			var productid_check = db.Stationeries.FirstOrDefault(r => r.productName == productName);

			string sqlQuery = "Update Requests SET Price = @Price, Note = @Note, Status = @Status where Role_ID = @Role_ID and productID = @productID and Unit = @Unit and Status = @Status_first";

			int role_id = rollid_check.Role_ID;
			int product_id = productid_check.productID;

			con.Open();
			SqlCommand cmd_nhaphang = new SqlCommand(sqlQuery, con);

			int status_update = Convert.ToInt32(Status) + 1;

			cmd_nhaphang.Parameters.AddWithValue("@Price", Price);
			cmd_nhaphang.Parameters.AddWithValue("@Status", status_update);
			cmd_nhaphang.Parameters.AddWithValue("@Role_ID", role_id);
			cmd_nhaphang.Parameters.AddWithValue("@Note", "");
			cmd_nhaphang.Parameters.AddWithValue("@productID", product_id);
			cmd_nhaphang.Parameters.AddWithValue("@Unit", Unit);
			cmd_nhaphang.Parameters.AddWithValue("@Status_first", Status);
			int check_nhaphang = cmd_nhaphang.ExecuteNonQuery();
			con.Close();

			if (check_nhaphang > 0)
			{
				TempData["Notify"] = "Provide Unit Price Succesfully";
				long price = Convert.ToInt64(Price);
				int status = Convert.ToInt16(Status);

				var requestid_check = from r in db.Requests
									  where r.Role_ID == role_id && r.productID == role_id && r.Price == price && r.Status == status_update
									  select r.Request_Id;

				foreach (var item in requestid_check)
				{
					int id_request = Convert.ToInt16(item);
					Logcheck_id(id_request, "Provide Unit Price");
				}
				db.SaveChanges();
				return RedirectToAction("Index_Group_Product");
			}
			return RedirectToAction("Index_Group_Product");
		}

		//giám đốc duyệt giá

		[Authorize(Roles = "manager, root")]
		public ActionResult Check_Price(string role, string product, string unit, string quantity, string price, string status, bool pause, string note)
		{
			Statuscheck();
			if (role == null || product == null || unit == null || quantity == null || price == null || status == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.role = role;
			ViewBag.product = product;
			ViewBag.unit = unit;
			ViewBag.quantity = quantity;
			ViewBag.price = price;
			ViewBag.status = status;
			ViewBag.pause = pause;
			ViewBag.note = note;
			ViewBag.tontien = Convert.ToInt64(quantity) * Convert.ToInt64(price);
			return View();
		}

		[Authorize(Roles = "manager, root")]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Check_Price(string RoleName, string productName, string Unit, string Status_First, string Status, bool approve, string Note)
		{
			Statuscheck();
			var rollid_check = db.Roles.FirstOrDefault(r => r.RoleName == RoleName);
			var productid_check = db.Stationeries.FirstOrDefault(r => r.productName == productName);

			string sqlQuery = "Update Requests SET Status = @Status, Pause = @Pause, Note = @Note where Role_ID = @Role_ID and productID = @productID and Unit = @Unit and Status = @Status_first";

			if (Status == "3")
			{
				return RedirectToAction("Index_Group_Product");
			}
			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index_Group_Product");
			}
			if (Status == "0")
			{
				con.Open();
				SqlCommand cmd_checkhang = new SqlCommand(sqlQuery, con);
				cmd_checkhang.Parameters.AddWithValue("@Status", Status_First);
				cmd_checkhang.Parameters.AddWithValue("@Pause", true);
				cmd_checkhang.Parameters.AddWithValue("@Note", "");
				cmd_checkhang.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
				cmd_checkhang.Parameters.AddWithValue("@productID", productid_check.productID);
				cmd_checkhang.Parameters.AddWithValue("@Unit", Unit);
				cmd_checkhang.Parameters.AddWithValue("@Status_first", Status_First);
				int check_checkhang = cmd_checkhang.ExecuteNonQuery();
				con.Close();

				if (check_checkhang == 1)
				{
					TempData["Notify"] = notify_successful;
					int status = Convert.ToInt16(Status_First);
					int role_id = rollid_check.Role_ID;

					var requestid_check = from r in db.Requests
										  where r.Role_ID == role_id && r.productID == role_id && r.Status == status
										  select r.Request_Id;

					foreach (var item in requestid_check)
					{
						int id_request = Convert.ToInt16(item);
						Logcheck_id(id_request, "Pause check money");
					}
					db.SaveChanges();
				}
				return RedirectToAction("Index_Group_Product");
			}

			//approved, refused
			
			if (Status == "2")
			{
				if (Note == "")
				{
					TempData["app_false"] = "The reason for the refusal has not been specified";
					return RedirectToAction("Index_Group_Product");
				}
				con.Open();
				SqlCommand cmd = new SqlCommand(sqlQuery, con);
				cmd.Parameters.AddWithValue("@Status", Status);
				cmd.Parameters.AddWithValue("@Pause", false);
				cmd.Parameters.AddWithValue("@Note", Note);
				cmd.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
				cmd.Parameters.AddWithValue("@productID", productid_check.productID);
				cmd.Parameters.AddWithValue("@Unit", Unit);
				cmd.Parameters.AddWithValue("@Status_first", Status_First);
				int check = cmd.ExecuteNonQuery();
				con.Close();
				if (check > 0)
				{
					TempData["Notify"] = notify_successful;
					int status = Convert.ToInt16(Status);
					int role_id = rollid_check.Role_ID;
					var requestid_check = from r in db.Requests
										  where r.Role_ID == role_id && r.productID == role_id && r.Status == status
										  select r.Request_Id;

					foreach (var item in requestid_check)
					{
						int id_request = Convert.ToInt16(item);
						Logcheck_id(id_request, "Refuse Price");
					}
					db.SaveChanges();
				}
				return RedirectToAction("Index_Group_Product");
			}

			else
			{

				con.Open();
				SqlCommand cmd = new SqlCommand(sqlQuery, con);
				cmd.Parameters.AddWithValue("@Status", Status);
				cmd.Parameters.AddWithValue("@Pause", false);
				cmd.Parameters.AddWithValue("@Note", "");
				cmd.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
				cmd.Parameters.AddWithValue("@productID", productid_check.productID);
				cmd.Parameters.AddWithValue("@Unit", Unit);
				cmd.Parameters.AddWithValue("@Status_first", Status_First);
				int check = cmd.ExecuteNonQuery();
				con.Close();
				if (check > 0)
				{
					TempData["Notify"] = notify_successful;
					int status = Convert.ToInt16(Status);
					int role_id = rollid_check.Role_ID;
					var requestid_check = from r in db.Requests
										  where r.Role_ID == role_id && r.productID == role_id && r.Status == status
										  select r.Request_Id;
					
					foreach (var item in requestid_check)
					{
						int id_request = Convert.ToInt16(item);
						Logcheck_id(id_request, "Check price successfully");
						
					}

					db.SaveChanges();
				}



				return RedirectToAction("Index_Group_Product");
			}
		}


		//accountant pay money for supplies

		[Authorize(Roles = "accounting_department, root")]
		public ActionResult Send_Money(string role, string product, string unit, string quantity, string price, string status, string note)
		{
			Statuscheck();
			if (role == null || product == null || unit == null || quantity == null || price == null || status == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.role = role;
			ViewBag.product = product;
			ViewBag.unit = unit;
			ViewBag.quantity = quantity;
			ViewBag.price = price;
			ViewBag.status = status;
			ViewBag.note = note;
			ViewBag.tontien = Convert.ToInt64(quantity) * Convert.ToInt64(price);

			return View();
		}

		[Authorize(Roles = "accounting_department, root")]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Send_Money(string RoleName, string productName, string Unit, string Status, bool approve)
		{
			Statuscheck();
			var rollid_check = db.Roles.FirstOrDefault(r => r.RoleName == RoleName);
			var productid_check = db.Stationeries.FirstOrDefault(r => r.productName == productName);

			string sqlQuery = "Update Requests SET Status = @Status where Role_ID = @Role_ID and productID = @productID and Unit = @Unit and Status = @Status_first";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index_Group_Product");
			}

			int new_Status = Convert.ToInt16(Status) + 1;
			// refused
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", new_Status);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
			cmd.Parameters.AddWithValue("@productID", productid_check.productID);
			cmd.Parameters.AddWithValue("@Unit", Unit);
			cmd.Parameters.AddWithValue("@Status_first", Status);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check > 0)
			{
				TempData["Notify"] = notify_successful;
				int status = Convert.ToInt16(new_Status);
				int role_id = rollid_check.Role_ID;

				var requestid_check = from r in db.Requests
									  where r.Role_ID == role_id && r.productID == role_id && r.Status == new_Status
									  select r.Request_Id;

				foreach (var item in requestid_check)
				{
					int id_request = Convert.ToInt16(item);
					Logcheck_id(id_request, "Send Money");
				}
				db.SaveChanges();
				return RedirectToAction("Index_Group_Product");
			}
			return RedirectToAction("Index_Group_Product");
		}

		//Office_supplies_stationery buy proudct

		[Authorize(Roles = "Office_supplies_stationery, root")]
		public ActionResult Buy_Product(string role, string product, string unit, string quantity, string price, string status, string note)
		{
			Statuscheck();
			if (role == null || product == null || unit == null || quantity == null || price == null || status == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.role = role;
			ViewBag.product = product;
			ViewBag.unit = unit;
			ViewBag.quantity = quantity;
			ViewBag.price = price;
			ViewBag.status = status;
			ViewBag.note = note;
			ViewBag.tontien = Convert.ToInt64(quantity) * Convert.ToInt64(price);

			
			return View();
		}

		[Authorize(Roles = "Office_supplies_stationery, root")]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Buy_Product(string RoleName, string productName, string Unit, string Status, bool approve)
		{
			Statuscheck();

			var rollid_check = db.Roles.FirstOrDefault(r => r.RoleName == RoleName);
			var productid_check = db.Stationeries.FirstOrDefault(r => r.productName == productName);

			string sqlQuery = "Update Requests SET Status = @Status where Role_ID = @Role_ID and productID = @productID and Unit = @Unit and Status = @Status_first";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index_Group_Product");
			}

			int new_Status = Convert.ToInt16(Status) + 1;
			// từ chối
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", new_Status);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
			cmd.Parameters.AddWithValue("@productID", productid_check.productID);
			cmd.Parameters.AddWithValue("@Unit", Unit);
			cmd.Parameters.AddWithValue("@Status_first", Status);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check > 0)
			{
				TempData["Notify"] = notify_successful;
				int status = Convert.ToInt16(new_Status);
				int role_id = rollid_check.Role_ID;

				var requestid_check = from r in db.Requests
									  where r.Role_ID == role_id && r.productID == role_id && r.Status == new_Status
									  select r.Request_Id;

				foreach (var item in requestid_check)
				{
					int id_request = Convert.ToInt16(item);
					Logcheck_id(id_request, "Buy Product");
				}
				db.SaveChanges();
				return RedirectToAction("Index_Group_Product");
			}
			return RedirectToAction("Index_Group_Product");
		}

		//human_resources_department get product

		[Authorize(Roles = "human_resources_department, root")]
		public ActionResult Get_Product(string role, string product, string unit, string quantity, string status, string note)
		{
			Statuscheck();
			if (role == null || product == null || unit == null || quantity == null || status == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.role = role;
			ViewBag.product = product;
			ViewBag.unit = unit;
			ViewBag.quantity = quantity;
			ViewBag.status = status;
			ViewBag.note = note;

			return View();
		}

		[Authorize(Roles = "human_resources_department, root")]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Get_Product(string RoleName, string productName, string Unit, string Status, bool approve)
		{
			int i = 0;
			i++;
			Statuscheck();
			var rollid_check = db.Roles.FirstOrDefault(r => r.RoleName == RoleName);
			var productid_check = db.Stationeries.FirstOrDefault(r => r.productName == productName);

			string sqlQuery = "Update Requests SET Status = @Status where Role_ID = @Role_ID and productID = @productID and Unit = @Unit and Status = @Status_first";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index_Group_Product");
			}

			int new_Status = Convert.ToInt16(Status) + 1;
			// từ chối
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", new_Status);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Role_ID", rollid_check.Role_ID);
			cmd.Parameters.AddWithValue("@productID", productid_check.productID);
			cmd.Parameters.AddWithValue("@Unit", Unit);
			cmd.Parameters.AddWithValue("@Status_first", Status);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check > 0)
			{
				TempData["Notify"] = notify_successful;
				int status = Convert.ToInt16(new_Status);
				int role_id = rollid_check.Role_ID;

				var requestid_check = from r in db.Requests
									  where r.Role_ID == role_id && r.productID == role_id && r.Status == new_Status
									  select r.Request_Id;

				foreach (var item in requestid_check)
				{
					int id_request = Convert.ToInt16(item);
					Logcheck_id(id_request, "Human Department get product");
				}
				db.SaveChanges();
				return RedirectToAction("Index_Group_Product");
			}
			return RedirectToAction("Index_Group_Product");
		}

		//end group by 

		//members from departments get product
		public ActionResult GetProduct_Employee(int? id, int employee_id)
		{
			Statuscheck();
			var check_username = User.Identity.GetUserName();
			var check_user = db.Employees.Where(c => c.Email == check_username);
			int employee_check = (int)check_user.Select(r => r.Employee_ID).First();
			if (employee_id != employee_check)
			{
				TempData["app_false"] = "Wrong employee get prodcut";
				return RedirectToAction("Index");
			}
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult GetProduct_Employee(int Request_Id, string Status, bool approve)
		{
			Statuscheck();
			string sqlQuery = "Update Requests SET Status = @Status, Note = @Note where Request_Id = @Request_Id";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index");
			}

			// Confirm, từ chối
			int status_update = Convert.ToInt32(Status) + 1;
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", status_update);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Request_Id", Request_Id);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Notify"] = "Get Product Succesfully";
				Logcheck_id(Request_Id, "Get Product");
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}


		//members in payment departments (if necessary)
		public ActionResult Returns_Product(int? id, int employee_id)
		{
			Statuscheck();
			var check_username = User.Identity.GetUserName();
			var check_user = db.Employees.Where(c => c.Email == check_username);
			int employee_check = (int)check_user.Select(r => r.Employee_ID).First();
			if (employee_id != employee_check)
			{
				TempData["app_false"] = "Wrong Employee get Product";
				return RedirectToAction("Index");
			}
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Returns_Product(int Request_Id, string Status, bool approve)
		{
			Statuscheck();
			string sqlQuery = "Update Requests SET Status = @Status, Note = @Note where Request_Id = @Request_Id";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index");
			}

			// Confirm, từ chối
			int status_update = Convert.ToInt32(Status) + 1;
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", status_update);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Request_Id", Request_Id);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Notify"] = "Successfully returned the product, waiting for the Human resources department to pick up the goods ";
				Logcheck_id(Request_Id, "Returns Product");
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		//Human Resources Department get the Product Back

		[Authorize(Roles = "human_resources_department, root")]
		public ActionResult Taking_back(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}

		[Authorize(Roles = "human_resources_department, root")]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Taking_back(int Request_Id, string Status, bool approve)
		{
			Statuscheck();
			string sqlQuery = "Update Requests SET Status = @Status, Note = @Note where Request_Id = @Request_Id";

			if (approve == false)
			{
				TempData["app_false"] = "Request not confirmed";
				return RedirectToAction("Index");
			}

			// Confirm, từ chối
			int status_update = Convert.ToInt32(Status) + 1;
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Status", status_update);
			cmd.Parameters.AddWithValue("@Note", "");
			cmd.Parameters.AddWithValue("@Request_Id", Request_Id);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Notify"] = "Received Product Successfully ";
				Logcheck_id(Request_Id, "Get the Product Back");
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		//// GET: Requests/Edit/5
		//public ActionResult Edit(int? id)
		//{
		//	if (id == null)
		//	{
		//		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		//	}
		//	Request request = db.Requests.Find(id);
		//	if (request == null)
		//	{
		//		return HttpNotFound();
		//	}
		//	ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", request.Employee_ID);
		//	ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", request.Role_ID);
		//	ViewBag.productID = new SelectList(db.Stationeries, "productID", "productName", request.productID);
		//	return View(request);
		//}

		//// POST: Requests/Edit/5
		//// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		//// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit([Bind(Include = "Request_Id,Employee_ID,Role_ID,productID,Unit,Quantity,Price,Purpose,DateAdd,Status,Note")] Request request)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		db.Entry(request).State = EntityState.Modified;
		//		db.SaveChanges();
		//		return RedirectToAction("Index");
		//	}
		//	ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", request.Employee_ID);
		//	ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", request.Role_ID);
		//	ViewBag.productID = new SelectList(db.Stationeries, "productID", "productName", request.productID);
		//	return View(request);
		//}

		// GET: Requests/Delete/5

		public ActionResult Delete(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Request request = db.Requests.Find(id);
			if (request == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(request);
		}


		// POST: Requests/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Statuscheck();
			ck = "Delete";
			Request request = db.Requests.Find(id);
			request.Status = -2;
			db.Entry(request).State = EntityState.Modified;
			Logcheck_id(id, ck);
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
