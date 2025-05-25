using System;
using System.Collections.Generic;

namespace emlak.api.Models
{
    public class Property : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public int SquareMeters { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public bool IsForSale { get; set; }
        public bool IsForRent { get; set; }
        public string PropertyType { get; set; } // Apartment, House, Land, etc.
        public string Status { get; set; } // Active, Sold, Rented, etc.
        public bool IsApproved { get; set; } // İlan onay durumu
        public DateTime? ApprovedAt { get; set; } // Onaylanma tarihi
        public string? ApprovedBy { get; set; } // Onaylayan admin kullanıcısı
        public DateTime? RejectedAt { get; set; } // Reddedilme tarihi
        public string? RejectedBy { get; set; } // Reddeden admin kullanıcısı
        
        // Navigation properties
        public ICollection<PropertyFeature> Features { get; set; }
        public ICollection<PropertyImage> Images { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        // City and District relationships
        public int CityId { get; set; }
        public City City { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
    }
} 