//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeamGeneratorApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserInTeam
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int UserOnEventId { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual UserOnEvent UserOnEvent { get; set; }
    }
}
