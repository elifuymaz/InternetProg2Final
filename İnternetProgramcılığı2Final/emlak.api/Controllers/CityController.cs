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
    public class CityController : ControllerBase
    {
        private readonly CityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityController(CityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto<List<CityDto>>>> GetAll()
        {
            try
            {
                var cities = await _cityRepository.GetAllCitiesWithDistrictsAsync();
                var cityDtos = _mapper.Map<List<CityDto>>(cities);
                return Ok(ResultDto<List<CityDto>>.Success(cityDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<CityDto>>.Error($"Şehirler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto<CityDto>>> GetById(int id)
        {
            try
            {
                var city = await _cityRepository.GetCityWithDistrictsAsync(id);
                if (city == null)
                    return NotFound(ResultDto<CityDto>.Error("Şehir bulunamadı"));

                var cityDto = _mapper.Map<CityDto>(city);
                return Ok(ResultDto<CityDto>.Success(cityDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<CityDto>.Error($"Şehir getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpGet("with-properties")]
        public async Task<ActionResult<ResultDto<List<CityDto>>>> GetAllWithProperties()
        {
            try
            {
                var cities = await _cityRepository.GetCitiesWithPropertyCountAsync();
                var cityDtos = _mapper.Map<List<CityDto>>(cities);
                return Ok(ResultDto<List<CityDto>>.Success(cityDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<List<CityDto>>.Error($"Şehirler getirilirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<CityDto>>> Create([FromBody] CreateCityDto createCityDto)
        {
            try
            {
                if (await _cityRepository.ExistsByCodeAsync(createCityDto.Code))
                    return BadRequest(ResultDto<CityDto>.Error("Bu plaka kodu zaten kullanılıyor"));

                var city = _mapper.Map<City>(createCityDto);
                await _cityRepository.AddAsync(city);
                await _cityRepository.SaveChangesAsync();

                var cityDto = _mapper.Map<CityDto>(city);
                return Ok(ResultDto<CityDto>.Success(cityDto, "Şehir başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<CityDto>.Error($"Şehir oluşturulurken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<CityDto>>> Update(int id, [FromBody] UpdateCityDto updateCityDto)
        {
            try
            {
                var city = await _cityRepository.GetByIdAsync(id);
                if (city == null)
                    return NotFound(ResultDto<CityDto>.Error("Şehir bulunamadı"));

                if (await _cityRepository.ExistsByCodeAsync(updateCityDto.Code) && city.Code != updateCityDto.Code)
                    return BadRequest(ResultDto<CityDto>.Error("Bu plaka kodu zaten kullanılıyor"));

                _mapper.Map(updateCityDto, city);
                _cityRepository.Update(city);
                await _cityRepository.SaveChangesAsync();

                var cityDto = _mapper.Map<CityDto>(city);
                return Ok(ResultDto<CityDto>.Success(cityDto, "Şehir başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<CityDto>.Error($"Şehir güncellenirken bir hata oluştu: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDto<bool>>> Delete(int id)
        {
            try
            {
                var city = await _cityRepository.GetByIdAsync(id);
                if (city == null)
                    return NotFound(ResultDto<bool>.Error("Şehir bulunamadı"));

                _cityRepository.Remove(city);
                await _cityRepository.SaveChangesAsync();

                return Ok(ResultDto<bool>.Success(true, "Şehir başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultDto<bool>.Error($"Şehir silinirken bir hata oluştu: {ex.Message}"));
            }
        }
    }
} 