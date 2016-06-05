using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserOnEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Nullable<int> TeamId { get; set; }
        public int UserCategoryId { get; set; }
        public Nullable<int> VoteCounter { get; set; }

        public string UserPersonalName { get; set; }
    }
}
