using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;
using emlak.api.Models;
using emlak.api.DTOs;
using emlak.api.Repositories;

namespace emlak.api.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPropertyController : ControllerBase
    {
        private readonly PropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public AdminPropertyController(PropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetAll()
        {
            try
            {
                var properties = await _propertyRepository.GetAllAsync();
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Mülkler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("approved")]
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetApproved()
        {
            try
            {
                var properties = await _propertyRepository.GetApprovedPropertiesAsync();
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Onaylı mülkler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetPending()
        {
            try
            {
                var properties = await _propertyRepository.GetPendingPropertiesAsync();
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Onay bekleyen mülkler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("rejected")]
        public async Task<ActionResult<ResultDto<List<PropertyDto>>>> GetRejected()
        {
            try
            {
                var properties = await _propertyRepository.GetRejectedPropertiesAsync();
                var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);
                return Ok(ResultDto<List<PropertyDto>>.Success(propertyDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<PropertyDto>>.Error($"Reddedilen mülkler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<PropertyDto>>> GetById(int id)
        {
            try
            {
                var property = await _propertyRepository.GetPropertyWithDetailsAsync(id);
                if (property == null)
                    return NotFound(ResultDto<PropertyDto>.Error("Mülk bulunamadı"));

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResultDto<PropertyDto>>> Create([FromBody] CreatePropertyDto createPropertyDto)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var property = _mapper.Map<Property>(createPropertyDto);
                
                // Admin direkt onaylı olarak ekler
                property.IsApproved = true;
                property.Status = "Active";
                property.ApprovedAt = DateTime.UtcNow;
                property.ApprovedBy = adminId;
                property.UserId = adminId; // Admin kendi adına ekler

                await _propertyRepository.AddAsync(property);
                await _propertyRepository.SaveChangesAsync();

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto, "Mülk başarıyla oluşturuldu ve onaylandı"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk oluşturulurken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<ActionResult<ResultDto<PropertyDto>>> ApproveProperty(int id)
        {
            try
            {
                var property = await _propertyRepository.GetByIdAsync(id);
                if (property == null)
                    return NotFound(ResultDto<PropertyDto>.Error("Mülk bulunamadı"));

                property.IsApproved = true;
                property.Status = "Active";
                _propertyRepository.Update(property);
                await _propertyRepository.SaveChangesAsync();

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto, "Mülk başarıyla onaylandı"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk onaylanırken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<ActionResult<ResultDto<PropertyDto>>> RejectProperty(int id)
        {
            try
            {
                var property = await _propertyRepository.GetByIdAsync(id);
                if (property == null)
                    return NotFound(ResultDto<PropertyDto>.Error("Mülk bulunamadı"));

                property.Status = "Rejected";
                _propertyRepository.Update(property);
                await _propertyRepository.SaveChangesAsync();

                var propertyDto = _mapper.Map<PropertyDto>(property);
                return Ok(ResultDto<PropertyDto>.Success(propertyDto, "Mülk başarıyla reddedildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<PropertyDto>.Error($"Mülk reddedilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultDto<bool>>> Delete(int id)
        {
            try
            {
                var property = await _propertyRepository.GetByIdAsync(id);
                if (property == null)
                    return NotFound(ResultDto<bool>.Error("Mülk bulunamadı"));

                _propertyRepository.Remove(property);
                await _propertyRepository.SaveChangesAsync();

                return Ok(ResultDto<bool>.Success(true, "Mülk başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<bool>.Error($"Mülk silinirken bir hata oluştu: {ex.Message}"));
            }
        }
    }
} 