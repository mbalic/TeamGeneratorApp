using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class PositionVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Value { get; set; }
        public int CategoryId { get; set; }
    }
}
