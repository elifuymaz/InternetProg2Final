using System;
using System.Collections.Generic;

namespace emlak.api.DTOs
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DistrictDto> Districts { get; set; }
        public int PropertyCount { get; set; }
    }

    public class CreateCityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class UpdateCityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}