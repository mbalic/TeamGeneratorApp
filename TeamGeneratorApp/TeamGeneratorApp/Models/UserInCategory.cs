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
    
    public partial class UserInCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserInCategory()
        {
            this.UserOnEvent = new HashSet<UserOnEvent>();
        }
    
        public int Id { get; set; }
        public int UserInGroupId { get; set; }
        public int CategoryId { get; set; }
        public int Rating { get; set; }
        public bool Active { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual UserInGroup UserInGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserOnEvent> UserOnEvent { get; set; }
    }
}
