using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SecretSanta.Web.ViewModels
{
    public class GiftViewModel
    {
        public int Id { get; set; } = 0;

        [Required]
        [Display(Name = "Recipient")]
        public int UserId { get; set; } = 0;

        [Required]
        [Display(Name="Title")]
        public string Title { get; set; } = "";

        [Display(Name="Description")]
        public string? Description { get; set; } = "";

        [Display(Name="URL")]
        [Url]
        public string? URL { get; set; } = "";

        [Display(Name = "Priority")]
        [RegularExpression("^[0-9]+$")] // restrict to ints
        public int? Priority { get; set; } = 0;
    }
}
