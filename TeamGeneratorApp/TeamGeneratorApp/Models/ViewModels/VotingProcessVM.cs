using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class VotingProcessVM
    {
        public string VotingId { get; set; }
        public int UserVotingId { get; set; }

        public int UserOnEvent1_Id { get; set; }
        public string User1_PersonalName { get; set; }
        public string User1_Image { get; set; }
        public int User1_Score { get; set; }

        public int UserOnEvent2_Id { get; set; }
        public string User2_PersonalName { get; set; }
        public string User2_Image { get; set; }
        public int User2_Score { get; set; }
    }
}
