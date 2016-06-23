using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class CooperationVM
    {
        public int UserId { get; set; }

        [Display(Name = "User")]
        public string UserPersonalName { get; set; }

        public double? SuccessPercentage { get; set; }
        public int Appereances { get; set; }
    }
}
