using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TeamGeneratorApp.Helpers
{
    public static class MenuSecurity
    {
        public static bool AdminTabVisible
        {
            get
            {
                return
                    HttpContext.Current.User != null &&
                    HttpContext.Current.User.IsInRole("Admin");
            }
        }

        public static bool UserTabVisible
        {
            get
            {
                return
                    HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }
    }
}
