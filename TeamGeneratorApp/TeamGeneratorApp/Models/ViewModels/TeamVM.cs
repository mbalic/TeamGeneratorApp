using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class TeamVM
    {
        public int Id { get; set; }
        public int RatingSum { get; set; }
        public int GeneratorId { get; set; }
        public string Name { get; set; }
        public List<UserOnEventVM> Users { get; set; } 

    }
}
