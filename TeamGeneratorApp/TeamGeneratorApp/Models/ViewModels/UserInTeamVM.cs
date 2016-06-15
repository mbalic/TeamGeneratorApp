using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserInTeamVM
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int UserOnEventId { get; set; }
        public TeamVM Team { get; set; }
        public UserOnEventVM UserOnEvent { get; set; }
    }
}
