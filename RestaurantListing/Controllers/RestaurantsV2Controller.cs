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
    //[ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsV2Controller : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DishesController> _logger;
        private readonly IMapper _mapper;

        public RestaurantsV2Controller(IUnitOfWork unitOfWork, ILogger<DishesController> logger, IMapper mapper)
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



    }
}
