using System;

namespace emlak.api.Models
{
    public class PropertyFeature : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        
        // Navigation property
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
} 