using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class VotingVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int EventId { get; set; }
        public Nullable<System.DateTime> StartVoting { get; set; }
        public Nullable<System.DateTime> FinishVoting { get; set; }
        public bool Active { get; set; }

    }
}
