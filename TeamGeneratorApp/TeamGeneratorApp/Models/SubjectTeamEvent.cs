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
    
    public partial class SubjectTeamEvent
    {
        public int Id { get; set; }
        public int SubjectInPoolId { get; set; }
        public int EventId { get; set; }
        public int TeamId { get; set; }
    
        public virtual Event Event { get; set; }
        public virtual SubjectInPool SubjectInPool { get; set; }
        public virtual Team Team { get; set; }
    }
}