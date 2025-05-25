using System;

namespace emlak.api.Models
{
    public class PropertyImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        
        // Navigation property
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
} 