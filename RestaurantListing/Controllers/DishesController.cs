using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantListing.DTOs;
using RestaurantListing.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DishesController> _logger;
        private readonly IMapper _mapper;

        public DishesController(IUnitOfWork unitOfWork, ILogger<DishesController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDishes()
        {
            try
            {
                var dishes = await _unitOfWork.Dishes.GetAll(orderBy: o => o.OrderByDescending(d => d.Stars));
                var results = _mapper.Map<IList<DishDTO>>(dishes);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN METHOD {nameof(GetDishes)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDish(int id)
        {
            try
            {
                var dish = await _unitOfWork.Dishes.Get(expression: c => c.Id.Equals(id), include: c1 => c1.Include(c2 => c2.Restaurant));
                var results = _mapper.Map<DishDTO>(dish);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN {nameof(GetDish)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}
