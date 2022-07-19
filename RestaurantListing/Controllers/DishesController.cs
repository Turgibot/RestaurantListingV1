﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestaurantListing.Data;
using RestaurantListing.DTOs;
using RestaurantListing.Models;
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
        //[Authorize(Roles = "User", Policy = "PayingOnly")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDishes([FromQuery]PagingParams pagingParams )
        {
            //var dishes = await _unitOfWork.Dishes.GetAll(orderBy: o => o.OrderByDescending(d => d.Stars));
            //var dishes = await _unitOfWork.Dishes.GetAllPaginated(pagingParams, orderBy: o => o.OrderByDescending(d => d.Stars));
            var dishes = await _unitOfWork.Dishes.GetAllPaginatedImproved(pagingParams, orderBy: o => o.OrderByDescending(d => d.Stars));
            var results = _mapper.Map<IList<DishDTO>>(dishes);

            var metadata = new
            {
                dishes.TotalCount,
                dishes.PageSize,
                dishes.CurrentPage,
                dishes.TotalPages,
                dishes.HasNext,
                dishes.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            _logger.LogInformation($"successfully Returned paged dishes from Db");

            return Ok(results);
        }
        [HttpGet]
        [Route("{id:int}", Name ="GetDish")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _unitOfWork.Dishes.Get(expression: c => c.Id.Equals(id), include: c1 => c1.Include(c2 => c2.Restaurant));
            var results = _mapper.Map<DishDTO>(dish);
            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateDish([FromBody] CreateDishDTO dishDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateDish)}");
                return BadRequest(ModelState);
            }

            var dish = _mapper.Map<Dish>(dishDTO);
            await _unitOfWork.Dishes.Insert(dish);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetDish", new { id = dish.Id }, dish);

        }

        // [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDish(int id, [FromBody] UpdateDishDTO dishDTO)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateDish)}");
                return BadRequest(ModelState);
            }

            var dish = await _unitOfWork.Dishes.Get(q => q.Id == id);
            if (dish == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateDish)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(dishDTO, dish);
            _unitOfWork.Dishes.Update(dish);
            await _unitOfWork.Save();

            return NoContent();

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDish(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteDish)}");
                return BadRequest();
            }

            var dish = await _unitOfWork.Dishes.Get(q => q.Id == id);
            if (dish == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteDish)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Dishes.Delete(id);
            await _unitOfWork.Save();

            return StatusCode(StatusCodes.Status202Accepted, dish);

        }
    }
}
