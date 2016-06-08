using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class VotingVM
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int EventId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; }

        [Display(Name = "Starts")]
        public Nullable<System.DateTime> StartVoting { get; set; }

        [Display(Name = "Ends")]
        public Nullable<System.DateTime> FinishVoting { get; set; }
        public bool Active { get; set; }

        [Required]
        public int VotesPerUser { get; set; }

    }
}
