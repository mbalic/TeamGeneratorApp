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
        public int TeamId { get; set; }

        [Required]
        public int UserInCategoryId { get; set; }
        public Nullable<int> VoteCounter { get; set; }

        [Required]
        public int PositionId { get; set; }
        
        [Display(Name = "Position")]
        public string PositionName { get; set; }

        public string UserPersonalName { get; set; }
        public int Rating { get; set; }

        public string UserId { get; set; }

    }
}
