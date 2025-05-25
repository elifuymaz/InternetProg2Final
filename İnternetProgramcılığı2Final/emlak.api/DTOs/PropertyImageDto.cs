using System;

namespace emlak.api.DTOs
{
    public class PropertyImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PropertyId { get; set; }
    }

    public class CreatePropertyImageDto
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }

    public class UpdatePropertyImageDto
    {
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }
}