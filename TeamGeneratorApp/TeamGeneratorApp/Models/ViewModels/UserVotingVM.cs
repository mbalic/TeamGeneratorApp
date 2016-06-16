using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserVotingVM
    {
        public int Id { get; set; }
        public string VotingId { get; set; }
        public int VoteCounter { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Draws { get; set; }
        public int UserOnEventId { get; set; }
        public string UserPersonalName { get; set; }
    }
}
