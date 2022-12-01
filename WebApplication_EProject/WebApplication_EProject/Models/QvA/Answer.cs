using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject.Models.QvA
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required(ErrorMessage = "Please input answer question")]
        [StringLength(maximumLength: 200, ErrorMessage = "Please input from 2 to 200 characters", MinimumLength = 2)]
        [Display(Name = "Answer question")]
        public string answer_question { get; set; }

        public DateTime answer_date { get; set; }

        [Editable(false)]
        public int? Employee_ID { get; set; }

        [Editable(false)]
        public int? QuestionId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Question Question { get; set; }
    }
}