using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Web.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; } = 0;

        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        public override string ToString() => FirstName + " " + LastName;
    }
}
