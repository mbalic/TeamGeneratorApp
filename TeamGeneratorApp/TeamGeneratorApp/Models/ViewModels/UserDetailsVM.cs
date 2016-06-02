using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserDetailsVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<int> Age { get; set; }
        public string Sex { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
