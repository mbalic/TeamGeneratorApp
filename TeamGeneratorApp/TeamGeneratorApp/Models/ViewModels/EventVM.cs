using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class EventVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Start { get; set; }
        public Nullable<System.DateTime> Finish { get; set; }
        public int NumberOfTeams { get; set; }
        public int PoolId { get; set; }
    }
}
