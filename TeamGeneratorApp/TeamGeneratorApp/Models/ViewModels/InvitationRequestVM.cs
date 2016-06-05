using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class InvitationRequestVM
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string UserPersonalName { get; set; }
        public string UserEmail { get; set; }
    }
}
