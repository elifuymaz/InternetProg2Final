using System;
using System.Collections.Generic;

namespace emlak.api.DTOs
{
    public class DistrictDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CityId { get; set; }
        public CityDto City { get; set; }
        public int PropertyCount { get; set; }
    }

    public class CreateDistrictDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int CityId { get; set; }
    }

    public class UpdateDistrictDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int CityId { get; set; }
    }
}