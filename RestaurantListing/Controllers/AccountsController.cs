using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantListing.Data;
using RestaurantListing.DTOs;
using System;
using System.Threading.Tasks;

namespace RestaurantListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;

        public AccountsController(ILogger<AccountsController> logger, IMapper mapper, UserManager<ApiUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register
            ([FromBody]RegisterUserDTO regUserDto)
        {
            try
            {
                _logger.LogInformation($"Registration attempt for user email {regUserDto.Email}");
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                ApiUser user = _mapper.Map<ApiUser>(regUserDto);
                user.UserName = regUserDto.Email;
                user.PhoneNumber = regUserDto.MobileNumber;

                var result = await _userManager.CreateAsync(user, regUserDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                return StatusCode(StatusCodes.Status201Created, "❤ User Created Successfully");
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
