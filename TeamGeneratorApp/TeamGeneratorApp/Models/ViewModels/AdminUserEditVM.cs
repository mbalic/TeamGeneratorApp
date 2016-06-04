using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class AdminUserEditVM
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Is admin")]
        public bool IsAdmin { get; set; }

        [Required]
        public string Name { get; set; }

        //public string Password { get; set; }

        public string ImageUrl { get; set; }
    }
}
