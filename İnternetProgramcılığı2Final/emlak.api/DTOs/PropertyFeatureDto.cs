using System;

namespace emlak.api.DTOs
{
    public class PropertyFeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PropertyId { get; set; }
    }

    public class CreatePropertyFeatureDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class UpdatePropertyFeatureDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}