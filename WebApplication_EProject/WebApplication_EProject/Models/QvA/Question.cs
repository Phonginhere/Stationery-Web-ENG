
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication_EProject.Models.EmployeeModel;

namespace WebApplication_EProject.Models.QvA
{
    public class Question
    {

        [Key]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Please input question name")]
        [StringLength(maximumLength: 25, ErrorMessage = "Please input from 2 to 25 characters", MinimumLength = 2)]
        [Display(Name = "Question Name")]
        public string question_name { get; set; }

        [Required(ErrorMessage = "Please input question description")]
        [StringLength(maximumLength: 200, ErrorMessage = "Please input from 2 to 200 characters", MinimumLength = 2)]
        [Display(Name = "Question Description")]
        public string question_desc { get; set; }

        public int? Role_ID { get; set; }

        [Editable(false)]
        public DateTime question_date { get; set; }
        [Editable(false)]
        public DateTime question_edited { get; set; }

        [Editable(false)]
        public int? Employee_ID { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<Answer> Answer { get; set; }

    }
}