using System;
using System.Data.Entity;
using System.Linq;
using WebApplication_EProject.Models.EmployeeModel;
using WebApplication_EProject.Models.QvA;

namespace WebApplication_EProject.Models
{
    public class ModelContext : DbContext
    {
        public ModelContext() : base("name=ModelContext")       
        {
            
        }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<ResetPass> ResetPasses { get; set; }
        public virtual DbSet<Question> Question { get; set; }

		public System.Data.Entity.DbSet<WebApplication_EProject.Models.QvA.Answer> Answers { get; set; }

		public System.Data.Entity.DbSet<WebApplication_EProject.Models.Request.Stationery> Stationeries { get; set; }

		public System.Data.Entity.DbSet<WebApplication_EProject.Models.Request.Request> Requests { get; set; }

		public System.Data.Entity.DbSet<WebApplication_EProject.Models.Request.Log> Logs { get; set; }
	}
}