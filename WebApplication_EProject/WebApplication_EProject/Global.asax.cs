﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebApplication_EProject.Models;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<ModelContext>(new StudentDbInitializer());
        }

    }
    public class StudentDbInitializer : DropCreateDatabaseIfModelChanges<ModelContext>
    {
        protected override void Seed(ModelContext context)
        {
            //roles
            var roles = new List<Role>
        {
            new Role { Role_ID = 1, RoleName = "root" },
            new Role { Role_ID = 2, RoleName = "manager" },
            new Role { Role_ID = 3, RoleName = "human_resources_department" },
            new Role { Role_ID = 4, RoleName = "accounting_department" },
            new Role { Role_ID = 5, RoleName = "Office_supplies_stationery" },
            new Role { Role_ID = 6, RoleName = "head_of_production" },
            new Role { Role_ID = 7, RoleName = "production_staff" },
            new Role { Role_ID = 8, RoleName = "head_of_humanresources_office_accounting" }
        };

            //employee
            var employees = new List<Employee>
        {
            new Employee { Employee_ID = 1, Name = "Hải Phong Trần", Email = "inputemail@gmail.com", PhoneNum = "0312345678", DateCreated = DateTime.Now,
                DateEdited = DateTime.Now,  DOB = Convert.ToDateTime("2004-04-12 00:00:00.000"),
                Password = BCrypt.Net.BCrypt.HashPassword("@Phongloveweb123"), Role_ID = 1, Status = 1}

        };

            //insert into db
            roles.ForEach(s => context.Roles.Add(s));
            employees.ForEach(s => context.Employees.Add(s));
            context.SaveChanges();

        }
    }

}
