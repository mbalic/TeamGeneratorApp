using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class GroupDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public List<CategoryVM> Categories { get; set; }
        public List<UserInGroupVM> Users { get; set; }
        public List<InvitationVM> Invitations { get; set; } 

    }
}
