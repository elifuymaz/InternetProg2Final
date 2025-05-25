using AutoMapper;
using emlak.api.DTOs;
using emlak.api.Models;

namespace emlak.api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Property mappings
            CreateMap<Property, PropertyDto>();
            CreateMap<CreatePropertyDto, Property>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features.Select(f => new PropertyFeature
                {
                    Name = f.Name,
                    Value = f.Value,
                    CreatedAt = DateTime.UtcNow
                })))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Images will be handled separately

            CreateMap<UpdatePropertyDto, Property>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // PropertyFeature mappings
            CreateMap<PropertyFeature, PropertyFeatureDto>();
            CreateMap<CreatePropertyFeatureDto, PropertyFeature>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // City mappings
            CreateMap<City, CityDto>();
            CreateMap<CreateCityDto, City>();
            CreateMap<UpdateCityDto, City>();

            // District mappings
            CreateMap<District, DistrictDto>();
            CreateMap<CreateDistrictDto, District>();
            CreateMap<UpdateDistrictDto, District>();

            // User mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<RegisterModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));

            // PropertyImage mappings
            CreateMap<PropertyImage, PropertyImageDto>();
            CreateMap<CreatePropertyImageDto, PropertyImage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdatePropertyImageDto, PropertyImage>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 