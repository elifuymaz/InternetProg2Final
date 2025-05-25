using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace emlak.api.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public int SquareMeters { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public bool IsForSale { get; set; }
        public bool IsForRent { get; set; }
        public string PropertyType { get; set; }
        public string Status { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Related data
        public CityDto City { get; set; }
        public DistrictDto District { get; set; }
        public List<PropertyFeatureDto> Features { get; set; }
        public List<PropertyImageDto> Images { get; set; }
        public UserDto User { get; set; }
    }

    public class CreatePropertyDto
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
        public string PropertyType { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public List<CreatePropertyFeatureDto> Features { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class UpdatePropertyDto
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
        public string PropertyType { get; set; }
        public string Status { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
    }
}