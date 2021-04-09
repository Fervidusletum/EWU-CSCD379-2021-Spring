using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Web.ViewModels
{
    public class GroupViewModel
    {
        [Required]
        [Display(Name="Group Name")]
        public string Name { get; set; } = "";
    }
}
