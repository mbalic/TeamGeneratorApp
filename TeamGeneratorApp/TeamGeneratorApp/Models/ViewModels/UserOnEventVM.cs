using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserOnEventVM
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Nullable<int> TeamId { get; set; }

        [Required]
        public string UserId { get; set; }
        public Nullable<int> VoteCounter { get; set; }

        [Required]
        public int PositionId { get; set; }

        public string UserPersonalName { get; set; }
        //public string TeamName { get; set; }
    }
}
