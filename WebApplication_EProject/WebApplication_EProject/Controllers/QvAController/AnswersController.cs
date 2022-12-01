using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.QvA;

namespace WebApplication_EProject.Controllers.QvAController
{
    [Authorize]
    public class AnswersController : Controller
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
        // GET: Answers
        public ActionResult Index()
        {
            Statuscheck();
            var answers = db.Answers.Include(a => a.Employee);
            return View(answers.ToList());
        }

        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            Statuscheck();
            if (id == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create(int? questionId, int employeeId)
        {
            Statuscheck();
            if (questionId == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            var check_username = User.Identity.GetUserName();
            var check_user = db.Employees.Where(c => c.Email == check_username);
            int role_check = (int)check_user.Select(r => r.Role_ID).First();

            var comp_check = db.Question.FirstOrDefault(q => q.Employee_ID == employeeId);

            if (comp_check.Role_ID != role_check)
            {
                return RedirectToAction("Question_Index", "Questions");
            }
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name");
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnswerId,answer_question,answer_date,Employee_ID,QuestionId")] Answer answer)
        {
            Statuscheck();
            if (ModelState.IsValid)
            {
                var check_username = User.Identity.GetUserName();
                var check_user = db.Employees.Where(c => c.Email == check_username);
                int employ_check = (int)check_user.Select(r => r.Employee_ID).First();

                answer.Employee_ID = employ_check;
                answer.answer_date = DateTime.Now;

                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Question_Details", "Questions", new { id = answer.QuestionId });

            }

            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", answer.Employee_ID);
            return View(answer);
        }

        // GET: Answers/Edit/5
        [Authorize(Roles = "root, manager")]
        public ActionResult Edit(int? id)
        {
            Statuscheck();
            if (id == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", answer.Employee_ID);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "root, manager")]
        public ActionResult Edit([Bind(Include = "AnswerId,answer_question,answer_date,Employee_ID,QuestionId")] Answer answer)
        {
            Statuscheck();
            if (ModelState.IsValid)
            {
                var check_username = User.Identity.GetUserName();
                var check_user = db.Employees.Where(c => c.Email == check_username);
                int employ_check = (int)check_user.Select(r => r.Employee_ID).First();

                answer.Employee_ID = employ_check;
                answer.answer_date = DateTime.Now;
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Name", answer.Employee_ID);
            return View(answer);
        }

        // GET: Answers/Delete/5
        [Authorize(Roles = "root, manager")]
        public ActionResult Delete(int? id)
        {
            Statuscheck();
            if (id == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return RedirectToAction("Error_Nothing", "Home");
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "root, manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Statuscheck();
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
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
