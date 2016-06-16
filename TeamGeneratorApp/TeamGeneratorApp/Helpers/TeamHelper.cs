using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.Helpers
{
    public class TeamHelper
    {
        public int Id { get; set; }
        public int Strength { get; set; }
        public List<UserOnEvent> Users { get; set; }
    }
}
