using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    }
}
