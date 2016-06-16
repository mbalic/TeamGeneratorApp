using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserCategoryVM
    {
        public int Id { get; set; }
        //public string UserId { get; set; }
        public int UserInGroupId { get; set; }
        public string UserPersonalName { get; set; }
        public int CategoryId { get; set; }
        public int Rating { get; set; }
        public bool Active { get; set; }


        //public UserVM User { get; set; }
        //public CategoryVM Category { get; set; }
    }
}
