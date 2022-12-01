using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject.Models.Request
{
	public class Log
	{
		public int LogId { get; set; }

		[Editable(false)]
		public int? Request_Id { get; set; }

		[Editable(false)]
		public int? Role_ID { get; set; }

		[Editable(false)]
		public int? Employee_ID { get; set; }

		[StringLength(maximumLength: 30)]
		[Display(Name = "Log Name")]
		[Editable(false)]
		public String LogName { get; set; }

		[Display(Name = "Log Date")]
		[Editable(false)]
		public DateTime LogDate { get; set; }

		public virtual Request Request { get; set; }
		public virtual Role Role { get; set; }
		public virtual Employee Employee { get; set; }
	}
}