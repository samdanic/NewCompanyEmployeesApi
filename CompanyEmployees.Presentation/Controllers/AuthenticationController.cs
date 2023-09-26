using CompanyEmployees.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AuthenticationController(IServiceManager service)
        {
            _service = service; 
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody]UsersForRegistrationDto usersForRegistrationDto)
        {
            var result = await _service.AuthenticationService.RegisterUser(usersForRegistrationDto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UsersForAuthenticationDto user)
        {
            if (!await _service.AuthenticationService.ValidateUser(user))
            {
                return Unauthorized();
            }
            //var tokenDto = await _service.AuthenticationService.CreateToken(populateExp: true);
            //return Ok(tokenDto);
            return Ok(new { Token = await _service.AuthenticationService.CreateToken(populateExp:true) });
        }
    }
}
