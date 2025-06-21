using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionUserController : ControllerBase
    {
        private readonly IUserService _service;

        public ExtensionUserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> PostAsync([FromQuery] string userName, [FromQuery] string pass)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pass))
                return BadRequest("Username and password are required.");

            try
            {
                var user = await _service.AuthenticateAsync(userName, pass);
                if (user != null)
                {
                    var token = _service.Generate(user);
                    return Ok(token);
                }
                return Unauthorized("User does not exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("getUsersByIds")]
        public async Task<IActionResult> GetUsersByIdsAsync([FromBody] List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                return BadRequest("User ID list is required.");

            try
            {
                var users = await _service.GetUsersByIdsAsync(userIds);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("validUserPassword")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> ValidUserPasswordAsync([FromQuery] int id, [FromQuery] string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                bool response = await _service.ValidUserPasswordAsync(id,password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }       
    }
}
