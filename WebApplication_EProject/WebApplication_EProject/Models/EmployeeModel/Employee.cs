using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication_EProject.Models.QvA;
using WebApplication_EProject.Models.Request;

namespace WebApplication_EProject.Models.EmployeeModel
{
    public class Employee
    {
        [Key]
        [Display(Name = "Employee ID")]
        public int Employee_ID { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please input full name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Input full name from 2 to 50 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please input email")]
        [RegularExpression(@"^([A-Za-z0-9_.]+)\@+(gmail|outlook|icloud|hotmail)*(.com|.uk|.vn|.us)$",
            ErrorMessage = "Wrong email address Ex: nameemail@gmail|outlook|icloud|hotmail.com|uk|vn|us")]
        [StringLength(maximumLength: 70, ErrorMessage = "Input email 70 characters")]
        public string Email { get; set; }

        [Display (Name = "Phone Number")]
        [Required (ErrorMessage = "Please input phone number")]
        [RegularExpression(@"(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Please input +84 vietnamese phone type")]
        [StringLength(maximumLength: 10, ErrorMessage = "Phone number has 10 numbers")]
        public string PhoneNum { get; set; }

        [Editable(false)]
        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [Editable(false)]
        [DataType(DataType.DateTime)]
        public DateTime DateEdited { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Please input Password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%<>'^&*-/|;:=_+{}()[~`]).{6,}$", ErrorMessage = "Password contains: 1 UpperCase Letter, 1 LowerCase Letter, 1 Symbol and 1 Number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please assign Role")]
        public int? Role_ID { get; set; }

        [Range(minimum: 0, maximum: 1)]
        public int Status { get; set; }

        public virtual ICollection<ResetPass> ResetPass { get; set; }

        //public virtual ICollection<Request.Request> Request { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<Answer> Answer { get; set; }

        public ICollection<Log> Log { get; set; }
    }
}