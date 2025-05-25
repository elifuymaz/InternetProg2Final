using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using emlak.api.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using emlak.api.DTOs;
using emlak.api.Repositories;

namespace emlak.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PropertyController : ControllerBase
    {
        private readonly PropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private const int MaxImageCount = 5;
        private const string ImageFolder = "property-images";

        public PropertyController(
            PropertyRepository propertyRepository, 
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetMyProperties()
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var properties = await _propertyRepository.GetPropertiesByUserAsync(userId);
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Mülkler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<PropertyDto>>> GetById(int id)
        {
            try
            {
                var property = await _propertyRepository.GetPropertyWithDetailsAsync(id);
                
                if (property == null)
                    return NotFound(ResultDto<PropertyDto>.Error("İlan bulunamadı"));

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"İlan getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResultDto<PropertyDto>>> Create([FromForm] CreatePropertyDto createPropertyDto)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;

                // Resim sayısı kontrolü
                if (createPropertyDto.Images != null && createPropertyDto.Images.Count > MaxImageCount)
                {
                    return BadRequest(ResultDto<PropertyDto>.Error($"En fazla {MaxImageCount} resim yükleyebilirsiniz"));
                }

                var property = _mapper.Map<Property>(createPropertyDto);
                property.UserId = userId;
                property.IsApproved = false;
                property.Status = "Pending";

                // Özellikleri ekle
                if (createPropertyDto.Features != null)
                {
                    property.Features = createPropertyDto.Features.Select(f => new PropertyFeature
                    {
                        Name = f.Name,
                        Value = f.Value
                    }).ToList();
                }

                // Resimleri kaydet
                if (createPropertyDto.Images != null && createPropertyDto.Images.Any())
                {
                    var imageFolderPath = Path.Combine(_environment.WebRootPath, ImageFolder);
                    if (!Directory.Exists(imageFolderPath))
                    {
                        Directory.CreateDirectory(imageFolderPath);
                    }

                    property.Images = new List<PropertyImage>();
                    foreach (var image in createPropertyDto.Images)
                    {
                        if (image.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                            var filePath = Path.Combine(imageFolderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            property.Images.Add(new PropertyImage
                            {
                                ImageUrl = $"/{ImageFolder}/{fileName}",
                                IsMain = property.Images.Count == 0 // İlk resim ana resim olsun
                            });
                        }
                    }
                }

                await _propertyRepository.AddAsync(property);
                await _propertyRepository.SaveChangesAsync();

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto, "Mülk başarıyla oluşturuldu ve onay için gönderildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk oluşturulurken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDto<PropertyDto>>> Update(int id, [FromForm] UpdatePropertyDto updatePropertyDto)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                var property = await _propertyRepository.GetByIdAsync(id);
                
                if (property == null)
                    return NotFound(ResultDto<PropertyDto>.Error("Mülk bulunamadı"));

                if (property.UserId != userId)
                    return Forbid();

                if (property.IsApproved)
                    return BadRequest(ResultDto<PropertyDto>.Error("Onaylanmış mülkler güncellenemez"));

                _mapper.Map(updatePropertyDto, property);
                property.IsApproved = false;
                property.Status = "Pending";

                _propertyRepository.Update(property);
                await _propertyRepository.SaveChangesAsync();

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto, "Mülk başarıyla güncellendi ve onay için gönderildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk güncellenirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultDto<bool>>> Delete(int id)
        {
            try
            {
                    var userId = User.FindFirst("uid")?.Value;
                var property = await _propertyRepository.GetByIdAsync(id);
                
                if (property == null)
                    return NotFound(ResultDto<bool>.Error("Mülk bulunamadı"));

                if (property.UserId != userId)
                    return Forbid();

                // Resimleri fiziksel olarak sil
                if (property.Images != null)
                {
                    foreach (var image in property.Images)
                    {
                        var imagePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                _propertyRepository.Remove(property);
                await _propertyRepository.SaveChangesAsync();

                return Ok(ResultDto<bool>.Success(true, "Mülk başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<bool>.Error($"Mülk silinirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("approved")]
      
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetApprovedProperties()
        {
            try
            {
                var properties = await _propertyRepository.GetApprovedPropertiesAsync();
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Onaylı ilanlar getirilirken bir hata oluştu: {ex.Message}"));
            }
        }
    }
} 