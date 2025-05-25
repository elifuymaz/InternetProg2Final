using System;
using System.Collections.Generic;

namespace emlak.api.Models
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        
        // Navigation property
        public ICollection<District> Districts { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
} 