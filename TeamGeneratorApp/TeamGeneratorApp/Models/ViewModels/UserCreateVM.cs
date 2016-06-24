using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserCreateVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}