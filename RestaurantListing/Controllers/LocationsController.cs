﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantListing.Core.DTOs;
using RestaurantListing.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public LocationsController(ILogger<LocationsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var locations = await _unitOfWork.Locations.GetAll();
                var mapped = _mapper.Map<IList<LocationDTO>>(locations);
                return Ok(mapped);
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN METHOD {nameof(GetLocations)}");
                return BadRequest(ex);
            }
        }
    }
}
