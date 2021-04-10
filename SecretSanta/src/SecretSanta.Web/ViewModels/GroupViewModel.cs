using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Web.ViewModels
{
    public class GroupViewModel
    {
        public int Id { get; set; } = 0;

        [Required]
        [Display(Name="Group Name")]
        public string Name { get; set; } = "";
    }
}
