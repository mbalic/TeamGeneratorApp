using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGeneratorApp.Models.ViewModels
{
    public class UserInPoolVM
    {
        public int Id { get; set; }
        public int PoolId { get; set; }
        public string UserId { get; set; }
        public string Weight { get; set; }

        public AspNetUsers User { get; set; }

    }
}
