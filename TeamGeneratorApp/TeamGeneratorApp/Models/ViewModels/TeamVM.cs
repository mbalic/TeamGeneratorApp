using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class TeamVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GeneratorId { get; set; }
        public int Strength { get; set; }

        [Range(0, 100)]
        [Display(Name = "Success percentage")]
        public Nullable<double> SuccessPercentage { get; set; }

    }
}
