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
    
    public partial class Score
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Score()
        {
            this.ScoreHistory = new HashSet<ScoreHistory>();
        }
    
        public int Id { get; set; }
        public Nullable<decimal> Value { get; set; }
        public int SubjectInPoolId { get; set; }
    
        public virtual SubjectInPool SubjectInPool { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScoreHistory> ScoreHistory { get; set; }
    }
}
