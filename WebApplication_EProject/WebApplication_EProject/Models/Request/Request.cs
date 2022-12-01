using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject.Models.Request
{
	public class Request
	{
		[Key]
		public int Request_Id { get; set; }

		[Display(Name = "Employee: ")]
		[Editable(false)]
		public int? Employee_ID { get; set; }

		[Display(Name = "Department")]
		[Editable(false)]
		public int? Role_ID { get; set; }

		[Required(ErrorMessage = "The name of the product you want to select is required")]
		[Display(Name = "Product")]
		public int? productID { get; set; }

		[Required(ErrorMessage = "Requires a unit of measurement for the product you want to select ")]
		[Display(Name = "Unit (box/piece/piece/...)")]
		[StringLength(maximumLength: 10, ErrorMessage = "Requires character length from 2 to 10 ", MinimumLength = 2)]
		public String Unit { get; set; }

		[Required(ErrorMessage = "Requires the quantity of the product you need")]
		[Display(Name = "The number of products")]
		[Range(minimum: 1, maximum: 1000, ErrorMessage = "Quantity must be from 1 to 1000")]
		public int Quantity { get; set; }

		[Required(ErrorMessage = "Ask for the price of the product you need ")]
		[Display(Name = "Unit price")]
		[Range(minimum: 0, maximum: 10000000, ErrorMessage = "The product price must be from 1 to 10,000,000 VND")]
		public long Price { get; set; }

		[Required(ErrorMessage = "The request must specify the purpose you need")]
		[Display(Name = "Purpose")]
		[StringLength(maximumLength: 25, MinimumLength = 2, ErrorMessage = "Yêu cầu phải nhập độ dài kí tự từ 2 đến 25")]
		public String Purpose { get; set; }

		[Display(Name = "Request Time")]
		[Editable(false)]
		public DateTime DateAdd { get; set; }

		[Editable(false)]
		[Display (Name = "Status")]
		public int Status { get; set; }

		
		[Editable(false)]
		[Display(Name = "Pause Request")]
		public bool Pause { get; set; }

		[StringLength(maximumLength: 50, ErrorMessage = "Notes cannot be more than 50 characters")]
		public String Note { get; set; }

		public virtual Employee Employee { get; set; }
		public virtual Role Role { get; set; }
		public virtual Stationery Stationery { get; set; }

		public ICollection<Log> Log { get; set; }
	}
}