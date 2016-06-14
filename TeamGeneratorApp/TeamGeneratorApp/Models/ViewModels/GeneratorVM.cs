using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class GeneratorVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; }

        [Required]
        [Display(Name = "Number of teams")]
        [Range(1, 100)]
        public int NumberOfTeams { get; set; }
        public int CriteriaId { get; set; }

        [Display(Name = "Criteria")]
        public string CriteriaName { get; set; }
    }
}
