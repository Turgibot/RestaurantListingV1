using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestaurantListing.Core.DTOs;
using RestaurantListing.Data;
using RestaurantListing.Repositories;
using RestaurantListing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly CachingProperties _cachingProperties;

        private const string locationsCacheKey = "locationsList";
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public LocationsController(ILogger<LocationsController> logger, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache, CachingProperties cachingProperties)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _cachingProperties = cachingProperties;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLocations()
        {

            _logger.LogInformation("Trying to retrieve locations from cache");
            if (_cache.TryGetValue(locationsCacheKey, out IEnumerable<LocationDTO> locations))
            {
                _logger.LogInformation("locations found in cache");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(locationsCacheKey, out locations))
                    {
                        _logger.LogInformation("locations found in cache");
                    }
                    else
                    {

                        locations = await GetLocationsAndSetInCacheAsync();
                    }
                }
                finally
                {
                    semaphore.Release();
                }

            }
            return Ok(locations);
        }

        private async Task<IEnumerable<LocationDTO>> GetLocationsAndSetInCacheAsync()
        {

            _logger.LogInformation("locations not found in cache - Fetching from DB");
            var currLocations = await _unitOfWork.Locations.GetAll(orderBy: x => x.OrderBy(c => c.Address),
               include: l => l.Include(l1 => l1.Restaurant));

            var locations = _mapper.Map<IList<LocationDTO>>(currLocations);
            _cache.Set(locationsCacheKey, locations, _cachingProperties.CacheOptions);
            return locations;
        }



        [HttpGet("{id:int}")]
        //[Route("{id:int}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<IActionResult> GetLocation(int id)
        {
            try
            {
                //l => l.Include(l1 => l1.Restaurant);
                var location = await _unitOfWork.Locations.Get(
                    include: l => l.Include(l1 => l1.Restaurant),
                    expression: c => c.Id == id
                    );
                var mapped = _mapper.Map<LocationDTO>(location);
                return Ok(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN METHOD {nameof(GetLocation)}");
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDTO locationDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateLocation)}");
                return BadRequest(ModelState);
            }

            var location = _mapper.Map<Location>(locationDTO);

            try
            {
                await semaphore.WaitAsync();

                await _unitOfWork.Locations.Insert(location);
                await _unitOfWork.Save();

                await GetLocationsAndSetInCacheAsync();

            }
            finally
            {
                semaphore.Release();
            }
            return CreatedAtRoute("GetLocation", new { id = location.Id }, location);

        }




        // [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDTO locationDTO)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateLocation)}");
                return BadRequest(ModelState);
            }

            var location = await _unitOfWork.Locations.Get(q => q.Id == id);
            if (location == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateLocation)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(locationDTO, location);
            _unitOfWork.Locations.Update(location);
            await _unitOfWork.Save();

            return NoContent();

        }


        //[Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteLocation)}");
                return BadRequest();
            }

            var location = await _unitOfWork.Locations.Get(q => q.Id == id);
            if (location == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteLocation)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Locations.Delete(id);
            await _unitOfWork.Save();

            return StatusCode(StatusCodes.Status202Accepted, location);

        }
    }

}


