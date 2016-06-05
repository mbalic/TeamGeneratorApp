using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class EventVM
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Start { get; set; }
        public Nullable<System.DateTime> Finish { get; set; }

        [Required]
        [Display(Name = "Number of teams")]
        [Range(0, 100, ErrorMessage = "Number of teams must be positive number")]
        public int NumberOfTeams { get; set; }
         
    }
}
