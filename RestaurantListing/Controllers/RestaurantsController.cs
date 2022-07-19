using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantListing.Data;
using RestaurantListing.DTOs;
using RestaurantListing.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DishesController> _logger;
        private readonly IMapper _mapper;

        public RestaurantsController(IUnitOfWork unitOfWork, ILogger<DishesController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRestaurants()
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants.GetAll();
                var results = _mapper.Map<IList<RestaurantDTO>>(restaurants);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN METHOD {nameof(GetRestaurants)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRestaurant(int id)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.Get(expression: c => c.Id.Equals(id), include: c1 => c1.Include(c2 => c2.Location).Include(c3 => c3.Dishes));
                var results = _mapper.Map<RestaurantDTO>(restaurant);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN METHOD {nameof(GetRestaurant)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDTO restaurantDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateRestaurant)}");
                return BadRequest(ModelState);
            }

            var restaurant = _mapper.Map<Restaurant>(restaurantDTO);
            await _unitOfWork.Restaurants.Insert(restaurant);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetRestaurant", new { id = restaurant.Id }, restaurant);

        }




        // [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDTO restaurantDTO)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateRestaurant)}");
                return BadRequest(ModelState);
            }

            var restaurant = await _unitOfWork.Restaurants.Get(q => q.Id == id);
            if (restaurant == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateRestaurant)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(restaurantDTO, restaurant);
            _unitOfWork.Restaurants.Update(restaurant);
            await _unitOfWork.Save();

            return NoContent();

        }


        //[Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteRestaurant)}");
                return BadRequest();
            }

            var restaurant = await _unitOfWork.Restaurants.Get(q => q.Id == id);
            if (restaurant == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteRestaurant)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Restaurants.Delete(id);
            await _unitOfWork.Save();

            return StatusCode(StatusCodes.Status202Accepted, restaurant);

        }



    }
}
