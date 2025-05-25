using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using emlak.api.Models;
using emlak.api.DTOs;
using emlak.api.Repositories;

namespace emlak.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly DistrictRepository _districtRepository;
        private readonly IMapper _mapper;

        public DistrictController(DistrictRepository districtRepository, IMapper mapper)
        {
            _districtRepository = districtRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto<List<DistrictDto>>>> GetAll()
        {
            try
            {
                var districts = await _districtRepository.GetAllAsync();
                var districtDtos = _mapper.Map<List<DistrictDto>>(districts);
                return Ok(ResultDto<List<DistrictDto>>.Success(districtDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<DistrictDto>>.Error($"İlçeler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<DistrictDto>>> GetById(int id)
        {
            try
            {
                var district = await _districtRepository.GetDistrictWithCityAsync(id);
                if (district == null)
                    return NotFound(ResultDto<DistrictDto>.Error("İlçe bulunamadı"));

                var districtDto = _mapper.Map<DistrictDto>(district);
                return Ok(ResultDto<DistrictDto>.Success(districtDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<DistrictDto>.Error($"İlçe getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("by-city/{cityId}")]
        public async Task<ActionResult<ResultDto<List<DistrictDto>>>> GetByCity(int cityId)
        {
            try
            {
                var districts = await _districtRepository.GetDistrictsByCityAsync(cityId);
                var districtDtos = _mapper.Map<List<DistrictDto>>(districts);
                return Ok(ResultDto<List<DistrictDto>>.Success(districtDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<DistrictDto>>.Error($"İlçeler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("with-properties/{cityId}")]
        public async Task<ActionResult<ResultDto<List<DistrictDto>>>> GetWithProperties(int cityId)
        {
            try
            {
                var districts = await _districtRepository.GetDistrictsWithPropertyCountAsync(cityId);
                var districtDtos = _mapper.Map<List<DistrictDto>>(districts);
                return Ok(ResultDto<List<DistrictDto>>.Success(districtDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<DistrictDto>>.Error($"İlçeler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<DistrictDto>>> Create([FromBody] CreateDistrictDto createDistrictDto)
        {
            try
            {
                if (await _districtRepository.ExistsInCityAsync(createDistrictDto.CityId, createDistrictDto.Code))
                    return BadRequest(ResultDto<DistrictDto>.Error("Bu ilçe kodu bu ilde zaten kullanılıyor"));

                var district = _mapper.Map<District>(createDistrictDto);
                await _districtRepository.AddAsync(district);
                await _districtRepository.SaveChangesAsync();

                var districtDto = _mapper.Map<DistrictDto>(district);
                return Ok(ResultDto<DistrictDto>.Success(districtDto, "İlçe başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<DistrictDto>.Error($"İlçe oluşturulurken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<DistrictDto>>> Update(int id, [FromBody] UpdateDistrictDto updateDistrictDto)
        {
            try
            {
                var district = await _districtRepository.GetByIdAsync(id);
                if (district == null)
                    return NotFound(ResultDto<DistrictDto>.Error("İlçe bulunamadı"));

                if (await _districtRepository.ExistsInCityAsync(updateDistrictDto.CityId, updateDistrictDto.Code) 
                    && (district.CityId != updateDistrictDto.CityId || district.Code != updateDistrictDto.Code))
                    return BadRequest(ResultDto<DistrictDto>.Error("Bu ilçe kodu bu ilde zaten kullanılıyor"));

                _mapper.Map(updateDistrictDto, district);
                _districtRepository.Update(district);
                await _districtRepository.SaveChangesAsync();

                var districtDto = _mapper.Map<DistrictDto>(district);
                return Ok(ResultDto<DistrictDto>.Success(districtDto, "İlçe başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<DistrictDto>.Error($"İlçe güncellenirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<bool>>> Delete(int id)
        {
            try
            {
                var district = await _districtRepository.GetByIdAsync(id);
                if (district == null)
                    return NotFound(ResultDto<bool>.Error("İlçe bulunamadı"));

                _districtRepository.Remove(district);
                await _districtRepository.SaveChangesAsync();

                return Ok(ResultDto<bool>.Success(true, "İlçe başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<bool>.Error($"İlçe silinirken bir hata oluştu: {ex.Message}"));
            }
        }
    }
} 