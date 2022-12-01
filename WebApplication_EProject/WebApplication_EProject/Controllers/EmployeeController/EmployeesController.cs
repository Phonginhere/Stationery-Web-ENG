using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject.Controllers.EmployeeController
{
	[Authorize]
	public class EmployeesController : Controller
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
		// GET: Employees
		public ActionResult Index(string Status, string Role_ID, string SearchString)
		{
			Statuscheck();
			var item = from a in db.Employees select a;
			if (Role_ID == null || Status == null || SearchString == null)
			{
				goto jump; //nhảy
			}

			if (Role_ID != "" && SearchString != "" && Status != "")
			{
				int check_Stat = Convert.ToInt32(Status);
				int check_role = Convert.ToInt32(Role_ID);
				var employees = from r in db.Employees
								where r.Role_ID == check_role
		 && (r.Name.Contains(SearchString) || r.Email.Contains(SearchString))
		 && r.Status == check_Stat
								select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
			else if (Role_ID != "" && SearchString != "")
			{
				int check_role = Convert.ToInt32(Role_ID);
				var employees = from r in db.Employees
								where r.Role_ID == check_role
								&& (r.Name.Contains(SearchString) || r.Email.Contains(SearchString))
								select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
			else if (Status != "" && SearchString != "")
			{
				int check_Stat = Convert.ToInt32(Status);
				var employees = from r in db.Employees
								where (r.Name.Contains(SearchString) || r.Email.Contains(SearchString))
								 && r.Status == check_Stat
								select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
			else if (Status != "" && Role_ID != "")
			{
				int check_Stat = Convert.ToInt32(Status);
				int check_role = Convert.ToInt32(Role_ID);
				var employees = from r in db.Employees
								where r.Role_ID == check_role
								 && r.Status == check_Stat
								select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}


			if (Role_ID != "") //filter deparment
			{
				int check_role = Convert.ToInt32(Role_ID);
				var employees = from r in db.Employees where r.Role_ID == check_role select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
			else if (SearchString != "")
			{
				var employees = from r in db.Employees
								where r.Name.Contains(SearchString) || r.Email.Contains(SearchString)
								select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
			else if (Status != "") //filter status
			{
				int check_Stat = Convert.ToInt32(Status);
				var employees = from r in db.Employees where r.Status == check_Stat select r;
				ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
				return View(employees.ToList());
			}
		jump:

			if (TempData["Alert"] != null)
			{

				ViewBag.Alert = TempData["Alert"];
			}
			if (TempData["problem"] != null)
			{

				ViewBag.problem = TempData["problem"];
			}
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			return View(item);
		}

		// GET: Employees/Details/5
		public ActionResult Details(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			Employee employee = db.Employees.Find(id);
			if (employee == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(employee);
		}

		//CREATE------------------------------------------------------------------------------------------------
		// GET: Employee/Create
		[Authorize(Roles = "root, manager")]
		public ActionResult Create()
		{
			Statuscheck();
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			return View();
		}
		// POST: Employee/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "root, manager")]
		public ActionResult Create([Bind(Include = "Employee_ID,Status,Name,Email,PhoneNum,DateCreated,DateEdited,DOB,Password,Role_ID")] Employee employee)
		{
			Statuscheck();
			if (ModelState.IsValid)
			{
				var email_dup_check = from edc in db.Employees select edc.Email;
				var phone_dup_check = from edc in db.Employees select edc.PhoneNum;

				int check = 0;
				if (email_dup_check.Contains(employee.Email))
				{
					ViewBag.err_email = "Email already taken";
					check++;
				}

				if (phone_dup_check.Contains(employee.PhoneNum))
				{
					ViewBag.err_SDT = "Phone number already taken";
					check++;
				}

				if (employee.Password.Length > 16)
				{
					ViewBag.err_l = "Character string limited to 16 characters";
					check++;
				}

				if (employee.Role_ID == 1)
				{
					ViewBag.root = "Only one root account";
					check++;
				}
				if (check > 0)
				{
					ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
					return View(employee);
				}


				string passwordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password);

				employee.Password = passwordHash;
				employee.DateCreated = DateTime.Now;
				employee.DateEdited = DateTime.Now;
				db.Employees.Add(employee);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
			return View(employee);
		}

		//EDIT------------------------------------------------------------------------------------------------
		// GET: Employees/Edit/5
		[Authorize(Roles = "root, manager")]
		public ActionResult Edit(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}

			if (id == 1)
			{
				TempData["problem"] = "Do not fix root account";
				return RedirectToAction("Index");
			}

			Employee employee = db.Employees.Find(id);
			if (employee == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName", employee.Role_ID);
			return View(employee);
		}

		// POST: Employees/Edit/5
		String SqlCon = ConfigurationManager.ConnectionStrings["ModelContext"].ConnectionString;
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "root, manager")]
		public ActionResult Edit([Bind(Include = "Employee_ID,Status,Name,Email,PhoneNum,DateCreated,DateEdited,DOB,Password,Role_ID")] Employee employee)
		{
			Statuscheck();
			if (ModelState.IsValid)
			{
				var email_dup_check = from edc in db.Employees select edc.Email;
				var phone_dup_check = from edc in db.Employees select edc.PhoneNum;

				int check = 0;
				var self_check = db.Employees.FirstOrDefault(n => n.Employee_ID == employee.Employee_ID);

				if (email_dup_check.Contains(employee.Email))
				{
					if (self_check.Email == employee.Email) //phải check email trùng đó có phải là email của chính nhân viên được sửa ko
					{
						goto jump;
					}
					ViewBag.err_email = "Email already taken";
					check++;
				}

			jump: //nhảy vào đây

				if (phone_dup_check.Contains(employee.PhoneNum))
				{
					if (self_check.PhoneNum == employee.PhoneNum) //tự hiểu
					{
						goto another_jump;
					}
					ViewBag.err_SDT = "Phone number already taken";
					check++;
				}
			another_jump: //nhảy tiếp

				if (employee.Password.Length > 16)
				{
					ViewBag.err_l = "Character string limited to 16 characters";
					check++;
				}
				if (employee.Role_ID == 1)
				{
					ViewBag.root = "Only one root account";
					check++;
				}
				if (check > 0)
				{
					ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "RoleName");
					return View(employee);
				}


				
				var employee_check = db.Employees.FirstOrDefault(x => x.Employee_ID == employee.Employee_ID);

				string passwordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password);

				
				employee.DateEdited = DateTime.Now;
				employee.DateCreated = employee_check.DateCreated;
				

				var employee_exist = new Employee
				{
					Employee_ID = employee.Employee_ID,
					Status = employee.Status,
					DOB = employee.DOB,
					Email = employee.Email,
					PhoneNum = employee.PhoneNum,
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

		//DELETE------------------------------------------------------------------------------------------------
		// GET: Employees/Delete/5
		[Authorize(Roles = "root, manager")]
		public ActionResult Delete(int? id)
		{
			Statuscheck();
			if (id == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}

			var username = User.Identity.GetUserName();
			var check_user = db.Employees.FirstOrDefault(c => c.Email == username);

			if (id == 1)
			{
				TempData["problem"] = "Do not delete root";
				return RedirectToAction("Index");
			}
			else if (id == check_user.Employee_ID)
			{
				TempData["problem"] = "Do not delete yourself";
				return RedirectToAction("Index");
			}

			Employee employee = db.Employees.Find(id);
			if (employee == null)
			{
				return RedirectToAction("Error_Nothing", "Home");
			}
			return View(employee);
		}

		// POST: Employees/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "root, manager")]
		public ActionResult DeleteConfirmed(int id)
		{
			Statuscheck();
			Employee employee = db.Employees.Find(id);

			
			var logcheck = from l in db.Logs where l.Employee_ID == id select l;
			db.Logs.RemoveRange(logcheck);
			
			var Requestcheck = from l in db.Requests where l.Employee_ID == id select l;
			db.Requests.RemoveRange(Requestcheck);
			
			var answer_check = db.Question.FirstOrDefault(a => a.Employee_ID == id);
			if (answer_check != null)
			{
				var Answercheck = from l in db.Answers where l.QuestionId == answer_check.QuestionId select l;
				db.Answers.RemoveRange(Answercheck);
			}
			
			var Questioncheck = from l in db.Question where l.Employee_ID == id select l;
			db.Question.RemoveRange(Questioncheck);
			
			db.Employees.Remove(employee);
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

