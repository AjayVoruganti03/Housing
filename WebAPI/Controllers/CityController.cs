using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Interfaces;
using WebAPI.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Authorize]   
    public class CityController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public CityController(IUnitOfWork uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet("cities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCities()
        {
            var cities = await uow.CityRepository.GetCitiesAsync();
            var citiesDto = mapper.Map<IEnumerable<CityDto>>(cities);
            return Ok(citiesDto);
        }

        [HttpPost("post")]
        public async Task<IActionResult> AddCity(CityDto cityDto)
        {
            var city = mapper.Map<City>(cityDto);
            city.LastUpdatedBy = 1;
            city.LastUpdated = DateTime.Now;
            uow.CityRepository.AddCity(city);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
        {
            try
            {
                if (id != cityDto.Id)
                {
                    return BadRequest("Update Not Allowed");
                }

                var cityFromDb = await uow.CityRepository.FindCity(id);
                if (cityFromDb == null)
                {
                    return BadRequest("City not found");
                }

                cityFromDb.LastUpdatedBy = 1;
                cityFromDb.LastUpdated = DateTime.Now;
                mapper.Map(cityDto, cityFromDb);

                await uow.SaveAsync();
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            } 
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] JsonPatchDocument<City> cityToPatch)
        {
            var cityFromDb = await uow.CityRepository.FindCity(id);
            if (cityFromDb == null)
                return NotFound("City not found");

            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdated = DateTime.Now;
            
            cityToPatch.ApplyTo(cityFromDb, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await uow.SaveAsync();
            return StatusCode(200);
        }

        [HttpPut("updateCityName/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityUpdateDto cityDto)
        {
            var cityFromDb = await uow.CityRepository.FindCity(id);
            if (cityFromDb == null)
                return NotFound("City not found");

            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdated = DateTime.Now;
            mapper.Map(cityDto, cityFromDb);

            await uow.SaveAsync();
            return StatusCode(200);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            uow.CityRepository.DeleteCity(id);
            await uow.SaveAsync();
            return Ok(id);
        }
    }
}
