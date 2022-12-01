using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication_EProject.Models.Request
{
    public class Stationery
    {
        [Key]
        public int productID { get; set; }

        [Required(ErrorMessage = "Please input product Name")]
        [Display(Name = "Prouduct Name")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Please input from 3 to 50 characters")]
        public string productName { get; set; }

        [Required(ErrorMessage = "Please input manufacturer")]
        [Display(Name = "Manufacturer")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "Please input from 2 to 50 characters")]
        public string producer { get; set; }

        [StringLength(maximumLength: 50, ErrorMessage = "Please input from 2 to 50 characters for name file", MinimumLength = 2)]
        [Display(Name = "Image Path")]
        public string productImage { get; set; }

        public ICollection<Request> Request { get; set; }
    }
}
