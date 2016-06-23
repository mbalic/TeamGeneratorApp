using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class GeneratorDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }
        public int EventId { get; set; }

        [Display(Name = "Number of teams")]
        public int NumberOfTeams { get; set; }

        [Display(Name = "Criteria")]
        public string CriteriaName { get; set; }

        public bool IsGenerated { get; set; }
        public bool IsLocked { get; set; }

    }
}
