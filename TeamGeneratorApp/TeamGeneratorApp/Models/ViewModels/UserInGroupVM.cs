using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserInGroupVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int GroupId { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        
    }
}
