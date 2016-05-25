using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class AdminUserIndexVM
    {
        public string Id { get; set; }        
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
