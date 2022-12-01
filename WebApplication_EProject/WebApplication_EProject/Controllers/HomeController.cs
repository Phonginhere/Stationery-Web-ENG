using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.EmployeeModel;
using WebApplication_EProject.Models.Request;

namespace WebApplication_EProject.Controllers
{
	public class HomeController : Controller
	{
		ModelContext db = new ModelContext();

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

		[Authorize]
		public ActionResult Index(String Status_Check)
		{
			Statuscheck();
			var user = User.Identity.GetUserName();
			var check_user = db.Employees.FirstOrDefault(c => c.Email.Equals(user));
			int role_number = (int)check_user.Role_ID;

			//count other
			int count_employee = (from c in db.Employees select c.Employee_ID).Count();
			int count_stationery = (from s in db.Stationeries select s.productID).Count();

			//request count:
			int count_request_cancel = (from c in db.Requests where c.Status == -2 select c).Count();
			int count_request_refused = (from c in db.Requests where c.Status == -1 select c).Count();

			int count_donhang_newly_created = (from c in db.Requests where c.Status == 0 || c.Status == 1 select c).Count();
			int count_donhang_newly_created_production_staff = (from c in db.Requests where c.Status == 0 select c).Count();
			int count_request_supplies_buy_product = (from c in db.Requests where c.Status == 5 select c).Count();

			//group by
			int group_by_sendmoney = (from r in db.Requests
									 where r.Status == 4
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
									 }).Count();
			int group_by_need_addunitprice = (from r in db.Requests
									   where r.Status == 2
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
									   }).Count();

			int group_by_buy_product = (from r in db.Requests
									 where r.Status == 5
									 group r by new { r.Role.RoleName, r.Stationery.productName, r.Unit, r.Status, r.Pause, r.Note } into g
									 select new Group<string, Request>
									 {
										 KeyRole = g.Key.RoleName,
										 KeyProduct = g.Key.productName,
										 KeyUnit = g.Key.Unit,
										 KeyQuantity = g.Sum(l => l.Quantity).ToString(),
										 KeyPrice = g.Sum(l => l.Price).ToString(),
										 KeyStatus = g.Key.Status.ToString(),
										 KeyPause = g.Key.Pause.ToString(),
										 KeyNote = g.Key.Note,
										 Values = g
									 }).Count();
			int count_request_employee_havenot_receive_product = (from c in db.Requests where c.Status == 7 select c).Count();
			int count_request_employee_getproduct = (from c in db.Requests where c.Status == 8 select c).Count();
			int count_request_return_to_human_resources_department = (from c in db.Requests where c.Status == 9 select c).Count();
			int count_request_return_product = (from c in db.Requests where c.Status == 10 select c).Count();

			//display count employee to stationery
			ViewBag.count_employee = count_employee;
			ViewBag.count_stationery = count_stationery;

			//display count request:
			ViewBag.count_request_cancel = count_request_cancel;
			ViewBag.count_request_refused = count_request_refused;

			ViewBag.count_donhang_newly_created = count_donhang_newly_created;
			ViewBag.count_donhang_newly_created_production_staff = count_donhang_newly_created_production_staff;
			ViewBag.count_request_supplies_buy_product = count_request_supplies_buy_product;

			ViewBag.count_request_employee_havenot_receive_product = count_request_employee_havenot_receive_product;
			ViewBag.count_request_employee_getproduct = count_request_employee_getproduct;

			ViewBag.count_request_return_to_human_resources_department = count_request_return_to_human_resources_department;
			ViewBag.count_request_return_product = count_request_return_product;


			//display group by
			ViewBag.group_by_need_addunitprice = group_by_need_addunitprice;
			ViewBag.group_by_buy_product = group_by_buy_product;
			ViewBag.group_by_sendmoney = group_by_sendmoney;

			var requests = from r in db.Requests where r.Role_ID == role_number select r;

			//status check
			int n;
			bool isNumeric = int.TryParse(Status_Check, out n);

			if (Status_Check != null && isNumeric == true)
			{
				int check = Convert.ToInt32(Status_Check);
				requests = from r in db.Requests where r.Status.Equals(check) && r.Role_ID == role_number select r;
			}
			if (TempData["Error"] != null)
			{
				ViewBag.Error = TempData["Error"];
			}
			return View(requests);
		}

		public ActionResult Error_Nothing()
		{
			return View();
		}

		public ActionResult Login()
		{
			//begin cập nhật
			bool check_online = User.Identity.IsAuthenticated;
			if (check_online == true)
			{
				return RedirectToAction("Error_Nothing");
			}
			//end cập nhật
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(Employee user)
		{
			//kiểm tra nếu 1 trong 2 ô textbox ko nhập 
			if (user.Password == null || user.Email == null)
			{
				ViewBag.haha = "Missing Email or Password";
			}
			else
			{

				var account = db.Employees.FirstOrDefault(x => x.Email == user.Email);

				if (account == null)
				{
					ViewBag.haha = "User Login Details Failed!!";
				}
				else
				{
					bool pass_user_check = BCrypt.Net.BCrypt.Verify(user.Password, account.Password);
					if (pass_user_check == false)
					{
						ViewBag.haha = "User Login Details Failed!!";
					}
					else
					{
						if (account.Status == 0)
						{
							ViewBag.haha = "Account currently disabled";
						}
						else
						{
							FormsAuthentication.SetAuthCookie(user.Email, false);
							return RedirectToAction("Index");
						}
					}

				}
				return View();
			}
			return View();
		}
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("Login");
		}


		public ActionResult Forget()
		{
			if (TempData["err_wr_e"] != null)
			{
				ViewBag.err_wr_e = TempData["err_wr_e"];
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Forget(string email)
		{
			bool has = db.Employees.Any(cus => cus.Email == email);

			var emailcheck = db.Employees.SingleOrDefault(x => x.Email == email);
			if (has == false)
			{
				//insert into db with random number
				TempData["err_wr_e"] = "Wrong Email";
				return RedirectToAction("Forget");
			}
			else if (emailcheck.Employee_ID == 1)
			{
				TempData["err_wr_e"] = "Không được phép chính sủa tài khoản root";
				return RedirectToAction("Forget");
			}

			//create random number
			Random _random = new Random();
			int num = _random.Next(100000, 999999);

			//send email with token
			MailMessage Msg = new MailMessage();
			Msg.From = new MailAddress("eprojectaptech20@gmail.com", "The reseter");// replace with valid value
			Msg.Subject = "Recover Password";
			Msg.To.Add(email); //replace with correct values
			Msg.Body = "The Code for reset password is: " + num;
			Msg.IsBodyHtml = true;
			//Msg.Priority = MailPriority.High;
			SmtpClient smtp = new SmtpClient();
			smtp.Host = "smtp.gmail.com";
			smtp.Port = 587;
			smtp.Credentials = new System.Net.NetworkCredential("eprojectaptech20@gmail.com", "<aptechloveproject17");// replace with valid value
			smtp.EnableSsl = true;
			smtp.Timeout = 20000;

			smtp.Send(Msg);

			// begin upload to db
			ResetPass ac = new ResetPass();
			ac.PassCodeNum = num;
			ac.Employee_ID = emailcheck.Employee_ID;
			db.ResetPasses.Add(ac);
			db.SaveChanges();
			// end upload to db

			TempData["check"] = email;
			return RedirectToAction("ResetByCode");
		}

		public ActionResult ResetByCode()
		{
			if (TempData["check"] != null)
			{
				ViewBag.check = TempData["check"];
			}
			if (TempData["err_wr_e"] != null)
			{
				ViewBag.err_wr_e = TempData["err_wr_e"];
			}
			return View();
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public ActionResult ResetByCode(String email, String code)
		{
			try
			{
				long coden = Convert.ToInt64(code);
				bool has = db.ResetPasses.Any(cus => cus.PassCodeNum == coden);
				var codecheck = db.ResetPasses.SingleOrDefault(x => x.PassCodeNum == coden);

				if (has == false)
				{
					TempData["err_wr_e"] = "Wrong Code";
					return RedirectToAction("ResetByCode");

				}
				ResetPass resetpass = db.ResetPasses.Find(codecheck.ResetPass_ID);
				db.ResetPasses.Remove(resetpass);
				db.SaveChanges();
				TempData["email_see"] = email;

				return RedirectToAction("ResetPassword");
			}
			catch
			{
				TempData["err_wr_e"] = "Wrong Code";
				return RedirectToAction("ResetByCode");
			}


		}

		public ActionResult ResetPassword()
		{
			if (TempData["email_see"] != null)
			{
				ViewBag.email_see = TempData["email_see"];
			}
			return View();
		}

		String SqlCon = ConfigurationManager.ConnectionStrings["ModelContext"].ConnectionString;
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(String email, Employee employee)
		{
			var emailcheck = db.Employees.SingleOrDefault(x => x.Email == email);

			string passwordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password);

			SqlConnection con = new SqlConnection(SqlCon);
			string sqlQuery = "Update Employees set Password = @Password where Employee_ID = @Employee_ID";
			con.Open();
			SqlCommand cmd = new SqlCommand(sqlQuery, con);
			cmd.Parameters.AddWithValue("@Password", passwordHash);
			cmd.Parameters.AddWithValue("@Employee_ID", emailcheck.Employee_ID);
			int check = cmd.ExecuteNonQuery();
			con.Close();
			if (check == 1)
			{
				TempData["Successful"] = "Insert Succesful";
				return RedirectToAction("Login");
			}
			return View();
		}

		//[Authorize(Roles = "Admin, Staff")]
		[Authorize]
		public ActionResult Account_Details()
		{
			Statuscheck();
			string email = User.Identity.GetUserName();
			if (email == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}

			var dbEntry = db.Employees.FirstOrDefault(acc => acc.Email == email);

			if (dbEntry == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(dbEntry);
		}

		// Get:
		[Authorize]
		public ActionResult Edit()
		{
			Statuscheck();
			string email = User.Identity.GetUserName();
			if (email == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			var dbEntry = db.Employees.FirstOrDefault(acc => acc.Email == email);
			if (dbEntry == null)
			{
				return HttpNotFound();
			}
			// begin cập nhạt
			if (dbEntry.Employee_ID == 1)
			{
				TempData["Error"] = "Không được phép chỉnh sửa tài khoản root";
				return RedirectToAction("Index");
			}
			// end cập nhạt
			if (TempData["old_check"] != null)
			{
				ViewBag.old_check = TempData["old_check"];

			}
			return View(dbEntry);
		}

		// POST: 
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Employee_ID,Name,Email,DateCreated,DateEdited,DOB,Password,Role_ID")] Employee employee)
		{
			Statuscheck();
			if (ModelState.IsValid)
			{
				var employee_check = db.Employees.FirstOrDefault(x => x.Employee_ID == employee.Employee_ID);


				string passwordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password);

				//begin gán cho thuộc tính của đối tượng
				employee.DateEdited = DateTime.Now;
				employee.DateCreated = employee_check.DateCreated;
				//begin gán cho thuộc tính của đối tượng

				var employee_exist = new Employee
				{
					Employee_ID = employee.Employee_ID,
					DOB = employee.DOB,
					Email = employee.Email,
					Name = employee.Name,
					Password = passwordHash,
					DateCreated = employee.DateCreated,
					DateEdited = employee.DateEdited,
					Role_ID = employee.Role_ID
				};

				db.Entry(employee_check).CurrentValues.SetValues(employee_exist);
				db.SaveChanges();
				return RedirectToAction("Index");

			}
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", employee.Role_ID);
			return View(employee);
		}


		[Authorize]
		public ActionResult Tutorial()
		{
			return View();
		}

	}
}