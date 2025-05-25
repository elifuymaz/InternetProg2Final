using System;
using System.Collections.Generic;

namespace emlak.api.Models
{
    public class District : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        
        // Navigation properties
        public int CityId { get; set; }
        public City City { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
} 